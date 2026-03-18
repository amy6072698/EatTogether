using EatTogether.Models.DTOs;
using EatTogether.Models.Infra;
using EatTogether.Models.Repositories;
using EatTogether.Models.ViewModels;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Services
{

	public interface IUserService
	{
		Task<Result> CreateAsync(UserCreateDto dto);
		Task<IEnumerable<UserListDto>> GetAllAsync(UserSearchDto dto, int currentUserId, bool canManage);
		Task<string> GetEmployeeNumberPreviewAsync();
		Task<UserEditDto?> GetForEditAsync(int id);
		Task<Result> ReinstateAsync(int id);
		Task<Result> ResignAsync(int id, int operatorId);
		Task<Result> UpdateAsync(int id, UserEditViewModel vm);
	}

	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepo;
		private readonly UserNumberGenerator _userNumberGenerator;

		public UserService(IUserRepository userRepo, UserNumberGenerator userNumberGenerator)
		{
			_userRepo = userRepo;
			_userNumberGenerator = userNumberGenerator;
		}
		public async Task<IEnumerable<UserListDto>> GetAllAsync(UserSearchDto dto, int currentUserId, bool canManage)
		{
			var userList = await _userRepo.GetAllAsync(dto);

			foreach (var item in userList)
			{
				// 只要有管理權限 (canManage)，就可以進入編輯頁面
				item.CanEdit = canManage;

				// 判斷是否可以執行「離職」處置
				item.CanResign = canManage && !item.IsDeleted && item.Id != currentUserId;

				// 判斷是否可以執行「復職」處置
				item.CanReinstate = canManage && item.IsDeleted;
			}

			return userList;
		}

		/* --------------------------------------------------------
           預產員工編號（供前端開 Modal 顯示用，不在 Transaction 內）
           注意：此編號僅供顯示，實際寫入時會在 Transaction 內重新產生
        -------------------------------------------------------- */
		public async Task<string> GetEmployeeNumberPreviewAsync()
		{
			var previewNumber = await _userRepo.GetLastEmployeeNumberByYearAsync(DateTime.Now.Year);
			return _userNumberGenerator.Generate(previewNumber);
		}

		public async Task<Result> CreateAsync(UserCreateDto dto)
		{
			// 業務驗證
			if (await _userRepo.IsAccountExistsAsync(dto.Account))
			{
				return Result.Fail("此帳號已存在，請使用其他帳號");
			}

			if (await _userRepo.IsEmailExistsAsync(dto.Email))
			{
				return Result.Fail("此 Email 已存在，請使用其他 Email");
			}

			if (!PasswordValidator.IsValid(dto.Password))
			{
				return Result.Fail("密碼至少 6 碼，且需包含英文與數字");
			}

			var hashedPassword = HashUtility.HashPassword(dto.Password);

			using var transaction = await _userRepo.BeginTransactionAsync();
			try
			{
				// 在 Transaction 內查詢最後編號（含 UPDLOCK/HOLDLOCK 鎖）
				await ExecuteInsertAsync(dto, hashedPassword);
				await transaction.CommitAsync();
				return Result.Success();

			}
			catch (DbUpdateException)
			{
				await transaction.RollbackAsync();
				return await RetryCreateAsync(dto, hashedPassword);
			}
			catch
			{
				await transaction.RollbackAsync();
				return Result.Fail("新增員工失敗，請稍後再試");
			}


		}

		/* --------------------------------------------------------
           新增員工重試（僅重試一次，重新產生員工編號）
        -------------------------------------------------------- */
		private async Task<Result> RetryCreateAsync(UserCreateDto dto, string hashedPassword)
		{
			using var transaction = await _userRepo.BeginTransactionAsync();
			try
			{
				await ExecuteInsertAsync(dto, hashedPassword);
				await transaction.CommitAsync();
				return Result.Success();
			}
			catch
			{
				await transaction.RollbackAsync();
				return Result.Fail("新增員工失敗，請重試");
			}
		}

		// 產生員工編號並寫入資料庫（必須在 Transaction 內呼叫）
		private async Task ExecuteInsertAsync(UserCreateDto dto, string hashedPassword)
		{
			var lastEmpNo = await _userRepo.GetLastEmployeeNumberByYearAsync(DateTime.Now.Year);
			var employeeNumber = _userNumberGenerator.Generate(lastEmpNo);

			var insertDto = new UserInsertDto
			{
				EmployeeNumber = employeeNumber,
				Name = dto.Name,
				Account = dto.Account,
				HashedPassword = hashedPassword,
				Email = dto.Email,
				Phone = dto.Phone,
				HireDate = dto.HireDate,
				IsActive = dto.IsActive,
				MustChangePassword = dto.Password == employeeNumber,
				RoleIds = dto.RoleIds
			};

			await _userRepo.InsertAsync(insertDto);
		}


		public async Task<UserEditDto?> GetForEditAsync(int id)
		{
			return await _userRepo.GetForEditAsync(id);
		}

		public async Task<Result> UpdateAsync(int id, UserEditViewModel vm)
		{
			// 確認員工存在
			var user = await _userRepo.GetForEditAsync(id);
			if (user == null)
			{
				return Result.Fail("找不到員工資料");
			}

			// 業務驗證：帳號唯一性（排除自己）
			if (await _userRepo.IsAccountExistsAsync(vm.Account, excludeId: id))
			{
				return Result.Fail("此帳號已存在，請使用其他帳號");
			}


			// 業務驗證：Email 唯一性（排除自己）
			if (await _userRepo.IsEmailExistsAsync(vm.Email, excludeId: id))
			{
				return Result.Fail("此 Email 已存在，請使用其他 Email");
			}

			string? hashedPassword = null;
			bool? mustChangePassword = null;

			// 密碼有填才處理
			if (!string.IsNullOrWhiteSpace(vm.Password))
			{
				if (!PasswordValidator.IsValid(vm.Password))
				{
					return Result.Fail("密碼至少 6 碼，且需包含英文與數字");
				}

				hashedPassword = HashUtility.HashPassword(vm.Password);
				mustChangePassword = vm.Password == user.EmployeeNumber;  // 明文比對
			}

			var updateDto = new UserUpdateDto
			{
				Id = id,
				Name = vm.Name,
				Account = vm.Account,
				HashedPassword = hashedPassword,
				Email = vm.Email,
				Phone = vm.Phone,
				HireDate = vm.HireDate,
				IsActive = vm.IsActive,
				MustChangePassword = mustChangePassword,
				RoleIds = vm.RoleIds
			};

			using var transaction = await _userRepo.BeginTransactionAsync();
			try
			{
				await _userRepo.UpdateAsync(updateDto);
				await _userRepo.UpdateUserRolesAsync(id, vm.RoleIds);
				await transaction.CommitAsync();
				return Result.Success();
			}
			catch
			{
				await transaction.RollbackAsync();
				return Result.Fail("更新員工失敗，請稍後再試");
			}
		}

		public async Task<Result> ResignAsync(int id, int operatorId)
		{
			// 禁止對自身帳號執行離職
			if (id == operatorId)
			{
				return Result.Fail("無法對自身帳號執行離職處理");
			}

			var user = await _userRepo.GetForEditAsync(id);
			if (user == null)
			{
				return Result.Fail("找不到員工資料");
			}

			await _userRepo.ResignAsync(id);
			return Result.Success();
		}

		public async Task<Result> ReinstateAsync(int id)
		{
			var user = await _userRepo.GetForEditAsync(id);
			if (user == null)
			{
				return Result.Fail("找不到員工資料");
			}

			await _userRepo.ReinstateAsync(id);
			return Result.Success();
		}
	}
}
