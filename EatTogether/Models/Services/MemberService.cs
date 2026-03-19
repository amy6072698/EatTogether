using EatTogether.Models.DTOs;
using EatTogether.Models.Infra;
using EatTogether.Models.Repositories;

namespace EatTogether.Models.Services
{
	public interface IMemberService
	{
		Task<IEnumerable<MemberListDto>> GetAllAsync(MemberSearchDto search);
		Task<MemberDetailDto?> GetDetailAsync(int id);
		Task<Result> BlacklistAsync(int id, string? reason);
		Task<Result> UnblacklistAsync(int id);
	}

	public class MemberService : IMemberService
	{
		private readonly IMemberRepository _repo;

		public MemberService(IMemberRepository repo)
		{
			_repo = repo;
		}

		public async Task<IEnumerable<MemberListDto>> GetAllAsync(MemberSearchDto search)
		{
			return await _repo.GetAllAsync(search);
		}

		public async Task<MemberDetailDto?> GetDetailAsync(int id)
		{
			return await _repo.GetByIdAsync(id);
		}

		public async Task<Result> BlacklistAsync(int id, string? reason)
		{
			var member = await _repo.GetByIdAsync(id);
			if (member is null)
				return Result.Fail("找不到該會員。");

			if (member.IsDeleted)
				return Result.Fail("已刪除的會員無法執行黑名單操作。");

			if (member.IsBlacklisted)
				return Result.Fail("該會員已在黑名單中。");

			await _repo.UpdateBlacklistAsync(id, true, reason);
			return Result.Success();
		}

		public async Task<Result> UnblacklistAsync(int id)
		{
			var member = await _repo.GetByIdAsync(id);
			if (member is null)
				return Result.Fail("找不到該會員。");

			if (member.IsDeleted)
				return Result.Fail("已刪除的會員無法執行黑名單操作。");

			if (!member.IsBlacklisted)
				return Result.Fail("該會員不在黑名單中。");

			await _repo.UpdateBlacklistAsync(id, false, null);
			return Result.Success();
		}
	}
}
