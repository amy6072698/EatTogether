using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Repositories
{
	public interface IMemberRepository
	{
		Task<IEnumerable<MemberListDto>> GetAllAsync(MemberSearchDto search);
	}

	public class MemberRepository : IMemberRepository
	{
		private readonly EatTogetherDBContext _context;

		public MemberRepository(EatTogetherDBContext context)
		{
			_context = context;
		}


		// 查詢會員列表（含篩選 / 排序）
		public async Task<IEnumerable<MemberListDto>> GetAllAsync(MemberSearchDto search)
		{
			var query = _context.Members.AsNoTracking();

			// 文字搜尋
			if (!string.IsNullOrWhiteSpace(search.Name))
				query = query.Where(m => m.Name.Contains(search.Name.Trim()));

			if (!string.IsNullOrWhiteSpace(search.Account))
				query = query.Where(m => m.Account.Contains(search.Account.Trim()));

			if (!string.IsNullOrWhiteSpace(search.Email))
				query = query.Where(m => m.Email.Contains(search.Email.Trim()));

			if (!string.IsNullOrWhiteSpace(search.Phone))
				query = query.Where(m => m.Phone != null && m.Phone.Contains(search.Phone.Trim()));

			// 狀態篩選
			query = search.Status switch
			{
				"Normal" => query.Where(m => m.IsConfirmed && !m.IsBlacklisted && !m.IsDeleted),
				"Unconfirmed" => query.Where(m => !m.IsConfirmed && !m.IsDeleted),
				"Blacklisted" => query.Where(m => m.IsBlacklisted && !m.IsDeleted),
				"Deleted" => query.Where(m => m.IsDeleted),
				_ => query   // "All"
			};

			// 排序
			query = search.SortBy == "CreatedAt_Asc"
				? query.OrderBy(m => m.CreatedAt)
				: query.OrderByDescending(m => m.CreatedAt);

			return await query.Select(m => new MemberListDto
			{
				Id = m.Id,
				Name = m.Name,
				Account = m.Account,
				Email = m.Email,
				Phone = m.Phone,
				BirthDate = m.BirthDate,
				CreatedAt = m.CreatedAt,
				IsConfirmed = m.IsConfirmed,
				IsBlacklisted = m.IsBlacklisted,
				IsDeleted = m.IsDeleted,
				DeletedAt = m.DeletedAt,
				BlacklistReason = m.BlacklistReason,
			}).ToListAsync();
		}

	}
}
