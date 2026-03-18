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

	public interface IRoleService
	{
		Task<Result> CreateAsync(RoleCreateDto dto);
		Task<IEnumerable<RoleListDto>> GetAllAsync();
		Task<RoleCreateViewModel_Data> GetCreateFormDataAsync();
		Task<RoleOverviewDto> GetOverviewAsync();
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
	}
}
