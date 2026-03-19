using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Repositories
{
	public interface IFunctionRepository
	{
		/// <summary>取得全部功能權限（供新增/編輯角色 Modal 的 Checkbox 卡片使用）</summary>
		Task<IEnumerable<FunctionDto>> GetAllAsync();
	}
	public class FunctionRepository : IFunctionRepository
	{
		private readonly EatTogetherDBContext _context;

		public FunctionRepository(EatTogetherDBContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<FunctionDto>> GetAllAsync()
		{
			var functions = await _context.Functions
				.AsNoTracking()
				.OrderBy(f => f.Id)
				.Select(f => new FunctionDto
				{
					Id = f.Id,
					Category = f.Category,
					FunctionName = f.FunctionName,
					DisplayName = f.DisplayName,
					Description = f.Description,
					IsOwnerOnly = f.IsOwnerOnly
				})
				.ToListAsync();

			return functions;
		}
	}
}
