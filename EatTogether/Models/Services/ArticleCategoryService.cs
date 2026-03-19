using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Services
{
	public class ArticleCategoryService
	{
		private readonly IArticleCategoryRepository _repo;
		private readonly EatTogetherDBContext _context;

		public ArticleCategoryService(IArticleCategoryRepository repo, EatTogetherDBContext context)
		{
			_repo = repo;
			_context = context;
		}

		public async Task CreateAsync(ArticleCategoryCreateDto dto) {
		
				await _repo.CreateAsync(dto);

		}

		public async Task<List<ArticleCategoryDto>> GetAllForIndexAsync()
		{
			return await _repo.GetAllAsync();
		}

		// 取得編輯用的資料
		public async Task<ArticleCategoryEditDto> GetEditByIdAsync(int id)
		{
			var result = await _repo.GetEditByIdAsync(id);
			if (result == null)
			{
				throw new Exception("找不到此文章類別");

			}
			return result;
		}

		// 編輯活動
		public async Task<Event_Article_ServiceResult<bool>> EditAsync(ArticleCategoryEditDto dto)
		{
			try
			{
				await _repo.EditAsync(dto);

				return Event_Article_ServiceResult<bool>.Ok(true);
			}
			catch (Exception ex)
			{
				return Event_Article_ServiceResult<bool>.Fail($"編輯失敗：{ex.Message}");
			}
		}


		// 檢查排序號碼是否重複
		public async Task<bool> IsSortOrderDuplicateAsync(int sortOrder, int? excludeId = null)
		{
			return await _context.ArticleCategories
				.AnyAsync(x => x.SortOrder == sortOrder && x.Id != excludeId);
		}

	}
}
