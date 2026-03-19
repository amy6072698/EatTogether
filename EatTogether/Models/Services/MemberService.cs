using EatTogether.Models.DTOs;
using EatTogether.Models.Repositories;

namespace EatTogether.Models.Services
{
	public interface IMemberService
	{
		Task<IEnumerable<MemberListDto>> GetAllAsync(MemberSearchDto search);
		Task<MemberDetailDto?> GetDetailAsync(int id);
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
	}
}
