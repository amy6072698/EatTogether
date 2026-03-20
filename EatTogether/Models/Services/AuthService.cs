using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.Infra;
using EatTogether.Models.Repositories;

namespace EatTogether.Models.Services
{
	public interface IAuthService
	{
		Task<Result<LoginDto>> ForceChangePasswordAsync(int userId, string newPassword);
		Task<Result> ForgotPasswordAsync(string email);
		Task<Result<LoginDto>> LoginAsync(string account, string password);
		Task<Result> ResetPasswordAsync(string token, string newPassword);
		Task<bool> ValidateResetTokenAsync(string token);
	}

	public class AuthService : IAuthService
	{
		private readonly IUserRepository _userRepo;
		private readonly IRoleRepository _roleRepo;
		private readonly IPasswordResetTokenRepository _tokenRepo;
		private readonly IRoleFunctionRepository _roleFuncRepo;
		private readonly IPasswordResetEmailService _emailService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AuthService(
			IUserRepository userRepo,
			IRoleRepository roleRepo,
			IPasswordResetTokenRepository tokenRepo,
			IRoleFunctionRepository roleFuncRepo,
			IPasswordResetEmailService emailService,
			IHttpContextAccessor httpContextAccessor)
		{
			_userRepo = userRepo;
			_roleRepo = roleRepo;
			_tokenRepo = tokenRepo;
			_roleFuncRepo = roleFuncRepo;
			_emailService = emailService;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<Result<LoginDto>> LoginAsync(string account, string password)
		{
			// 1. 用帳號查使用者
			var user = await _userRepo.GetByAccountAsync(account);

			// 2. 帳號不存在 or 密碼錯誤 → 一律回傳同一訊息（防帳號枚舉）
			if (user == null || !HashUtility.VerifyPassword(password, user.HashedPassword))
				return Result<LoginDto>.Fail("帳號或密碼錯誤");

			// 3. 已刪除的帳號 → 同樣回傳「帳號或密碼錯誤」（不透露帳號存在）
			if (user.IsDeleted)
				return Result<LoginDto>.Fail("帳號或密碼錯誤");

			// 4. 帳號停用 → 顯示明確訊息
			if (!user.IsActive)
				return Result<LoginDto>.Fail("此帳號已停用，請聯絡店長");

			var roleNames = await _roleRepo.GetRoleNamesByIdsAsync(user.RoleIds);

			var functionNames = (await _roleFuncRepo.GetFunctionNamesByRoleIdsAsync(user.RoleIds)).ToList();


			// 5. 驗證通過 → 組裝 LoginDto 回傳
			var loginDto = new LoginDto
			{
				UserId = user.Id,
				Account = user.Account,
				Name = user.Name,
				RoleIds = user.RoleIds,
				RoleNames = roleNames,
				FunctionNames = functionNames,
				MustChangePassword = user.MustChangePassword
			};

			return Result<LoginDto>.Success(loginDto);
		}

		public async Task<Result<LoginDto>> ForceChangePasswordAsync(int userId, string newPassword)
		{
			// 先確認使用者存在
			var user = await _userRepo.GetByIdAsync(userId);
			if (user == null) return Result<LoginDto>.Fail("使用者不存在");

			// 更新 HashedPassword
			var hashedPassword = HashUtility.HashPassword(newPassword);
			await _userRepo.UpdatePasswordAsync(userId, hashedPassword);

			// MustChangePassword 設為 0
			await _userRepo.SetMustChangePasswordAsync(userId, false);

			// 組裝 LoginDto
			var roleNames = await _roleRepo.GetRoleNamesByIdsAsync(user.RoleIds);
			var functionNames = (await _roleFuncRepo.GetFunctionNamesByRoleIdsAsync(user.RoleIds)).ToList();

			var loginDto = new LoginDto
			{
				UserId = user.Id,
				Account = user.Account,
				Name = user.Name,
				RoleIds = user.RoleIds,
				RoleNames = roleNames,
				FunctionNames = functionNames,
				MustChangePassword = false
			};

			return Result<LoginDto>.Success(loginDto);

		}

		public async Task<Result> ForgotPasswordAsync(string email)
		{
			var user = await _userRepo.GetByEmailAsync(email);

			// 查無此 Email 或使用者已刪除或停用 → 一律回傳成功（防帳號枚舉）
			if (user == null || user.IsDeleted || !user.IsActive) return Result.Success();

			// 將舊 Token 全部失效
			await _tokenRepo.InvalidatePreviousTokensAsync(user.Id);

			// 產生 32 碼 Token（Guid 去除符號）
			var tokenString = Guid.NewGuid().ToString("N");

			var tokenEntity = new PasswordResetToken
			{
				UserId = user.Id,
				Token = tokenString,
				IsUsed = false
			};

			await _tokenRepo.CreateAsync(tokenEntity);

			// 產生重設連結
			var request = _httpContextAccessor.HttpContext!.Request;
			var resetLink = $"{request.Scheme}://{request.Host}/Auth/ResetPassword?token={tokenString}";

			// 寄送 Email
			await _emailService.SendPasswordResetEmailAsync(email, resetLink);

			return Result.Success();
		}

		public async Task<bool> ValidateResetTokenAsync(string token)
		{
			var tokenEntity = await _tokenRepo.GetValidTokenAsync(token);
			return tokenEntity != null;
		}

		public async Task<Result> ResetPasswordAsync(string token, string newPassword)
		{
			// 驗證 Token
			var tokenEntity = await _tokenRepo.GetValidTokenAsync(token);
			if (tokenEntity == null) return Result.Fail("連結已失效或已使用，請重新申請");

			// 更新密碼
			var hashedPassword = HashUtility.HashPassword(newPassword);
			await _userRepo.UpdatePasswordAsync(tokenEntity.UserId, hashedPassword);

			// Token 標記為已使用（一次性）
			await _tokenRepo.MarkUsedAsync(tokenEntity.Id);

			return Result.Success();
		}



	}
}
