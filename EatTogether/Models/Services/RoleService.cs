using EatTogether.Models.DTOs;
using EatTogether.Models.Infra;
using EatTogether.Models.Repositories;

namespace EatTogether.Models.Services
{

	// 回傳給 Controller 的組合資料（含 AllFunctions + AllUsers），
	// 避免 Service 直接回傳 ViewModel（不跨層污染）
	public record RoleCreateViewModel_Data(
		IEnumerable<FunctionDto> AllFunctions,
		IEnumerable<UserForRoleDto> AllUsers);
	public record RoleEditViewModel_Data(
		RoleEditDto EditDto,
		IEnumerable<FunctionDto> AllFunctions,
		IEnumerable<UserForRoleDto> AllUsers);

	public interface IRoleService
	{
		Task<Result> CreateAsync(RoleCreateDto dto);
		Task<Result> DeleteAsync(int id);
		Task<IEnumerable<RoleListDto>> GetAllAsync();
		Task<RoleCreateViewModel_Data> GetCreateFormDataAsync();
		Task<RoleEditViewModel_Data?> GetEditFormDataAsync(int id);
		Task<RoleEditDto?> GetForEditAsync(int id);
		Task<RoleOverviewDto> GetOverviewAsync();
		Task<Result> UpdateAsync(int id, RoleEditDto dto);
	}

	public class RoleService : IRoleService
	{
		// 預設 6 個角色名稱（不可刪除）
		private static readonly HashSet<string> _defaultRoleNames = new(StringComparer.OrdinalIgnoreCase)
		{
			"店長", "副店長", "收銀員", "外場服務生", "內場廚師", "工讀生"
		};

		private readonly IRoleRepository _roleRepo;
		private readonly IFunctionRepository _functionRepo;

		public RoleService(IRoleRepository roleRepo, IFunctionRepository functionRepo)
		{
			_roleRepo = roleRepo;
			_functionRepo = functionRepo;
		}

		public async Task<IEnumerable<RoleListDto>> GetAllAsync()
		{
			return await _roleRepo.GetAllAsync();
		}

		// 權限總覽 Modal 資料
		public async Task<RoleOverviewDto> GetOverviewAsync()
		{
			return await _roleRepo.GetOverviewAsync();
		}

		// 取得新增角色 Modal 資料
		public async Task<RoleCreateViewModel_Data> GetCreateFormDataAsync()
		{
			var functions = await _functionRepo.GetAllAsync();
			var users = await _roleRepo.GetActiveUsersAsync();
			return new RoleCreateViewModel_Data(functions, users);
		}

		// 新增角色
		public async Task<Result> CreateAsync(RoleCreateDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.RoleName))
				return Result.Fail("角色名稱不可為空");

			if (await _roleRepo.IsNameDuplicateAsync(dto.RoleName))
				return Result.Fail($"角色名稱「{dto.RoleName}」已存在");

			// 後端過濾 IsOwnerOnly=1 的權限，防止透過 API 繞過前端限制
			var allFunctions = (await _functionRepo.GetAllAsync()).ToList();
			var allowedIds = allFunctions
				.Where(f => !f.IsOwnerOnly)
				.Select(f => f.Id)
				.ToHashSet();

			dto.FunctionIds = dto.FunctionIds
				.Where(fid => allowedIds.Contains(fid))
				.ToList();

			await _roleRepo.CreateAsync(dto);
			return Result.Success();
		}

		// 取得編輯角色 Modal 所需資料（含預填值）
		public async Task<RoleEditDto?> GetForEditAsync(int id)
		{
			return await _roleRepo.GetForEditAsync(id);
		}

		public async Task<RoleEditViewModel_Data?> GetEditFormDataAsync(int id)
		{
			var editDto = await _roleRepo.GetForEditAsync(id);
			if (editDto == null) return null;

			var functions = await _functionRepo.GetAllAsync();
			var users = await _roleRepo.GetActiveUsersAsync();
			return new RoleEditViewModel_Data(editDto, functions, users);
		}

		// 更新角色 
		public async Task<Result> UpdateAsync(int id, RoleEditDto dto)
		{
			var existing = await _roleRepo.GetForEditAsync(id);
			if (existing == null) return Result.Fail("找不到此角色");

			if (string.IsNullOrWhiteSpace(dto.RoleName))
				return Result.Fail("角色名稱不可為空");

			if (await _roleRepo.IsNameDuplicateAsync(dto.RoleName, excludeId: id))
				return Result.Fail($"角色名稱「{dto.RoleName}」已存在");

			var allFunctions = (await _functionRepo.GetAllAsync()).ToList();

			if (existing.RoleName == "店長")
			{
				// 店長必須保留 IsOwnerOnly 權限（不允許從 UI 移除）
				var ownerOnlyIds = allFunctions
					.Where(f => f.IsOwnerOnly)
					.Select(f => f.Id)
					.ToHashSet();
				dto.FunctionIds = dto.FunctionIds
					.Concat(ownerOnlyIds)
					.Distinct()
					.ToList();
			}
			else
			{
				// 非店長角色：過濾掉 IsOwnerOnly 項目
				var allowedIds = allFunctions
					.Where(f => !f.IsOwnerOnly)
					.Select(f => f.Id)
					.ToHashSet();
				dto.FunctionIds = dto.FunctionIds
					.Where(fid => allowedIds.Contains(fid))
					.ToList();
			}

			dto.Id = id;
			await _roleRepo.UpdateAsync(dto);
			return Result.Success();
		}

		// 刪除角色
		public async Task<Result> DeleteAsync(int id)
		{
			var role = await _roleRepo.GetForEditAsync(id);
			if (role == null) return Result.Fail("找不到此角色");

			// 預設 6 個角色不可刪除
			if (_defaultRoleNames.Contains(role.RoleName))
				return Result.Fail($"「{role.RoleName}」為系統預設角色，不允許刪除");

			await _roleRepo.DeleteAsync(id);
			return Result.Success();
		}
	}
}
