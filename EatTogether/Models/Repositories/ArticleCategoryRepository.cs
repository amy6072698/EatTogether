using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Repositories
{
	public class ArticleCategoryRepository : IArticleCategoryRepository
	{
		private readonly EatTogetherDBContext _context;

		public ArticleCategoryRepository(EatTogetherDBContext context)
		{
			_context = context;
		}

		public async Task CreateAsync(ArticleCategoryCreateDto dto)
		{
			var category = dto.ToEntity();

			_context.ArticleCategories.Add(category);
			await _context.SaveChangesAsync();
		}


		public async Task<List<ArticleCategoryDto>> GetAllAsync()
		{
			var data = await _context.ArticleCategories
				.AsNoTracking()
				.Select(e => new ArticleCategoryDto
				{
					Id = e.Id,
					Name = e.Name,
					SortOrder = e.SortOrder,
					IsEnabled = e.IsEnabled					
				})
				.ToListAsync();

			return data;
		}


		public async Task EditAsync(ArticleCategoryEditDto dto)
		{
			var entity = await _context.ArticleCategories.FindAsync(dto.Id);
			if (entity == null)
			{
				return;
			}

			entity.Id = dto.Id;
			entity.Name = dto.Name;
			entity.SortOrder = dto.SortOrder;
			entity.IsEnabled = dto.IsEnabled;

			await _context.SaveChangesAsync();
		}


		public async Task<ArticleCategoryEditDto> GetEditByIdAsync(int id)
		{
			var entity = await _context.ArticleCategories.FindAsync(id);

			if (entity == null) return null;

			return new ArticleCategoryEditDto
			{
				Id = id,
				Name= entity.Name,
				SortOrder = entity.SortOrder,
				IsEnabled = entity.IsEnabled
			};
		}
	}
}
