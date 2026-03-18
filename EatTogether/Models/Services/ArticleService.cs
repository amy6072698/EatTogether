using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Services
{
	public class ArticleService
	{
		private readonly IArticleRepository _repo;
		private readonly EatTogetherDBContext _context;

		public ArticleService(IArticleRepository repo, EatTogetherDBContext context)
		{
			_repo = repo;
			_context = context;
		}

		public async Task CreateAsync(ArticleCreateDto dto)
		{

			await _repo.CreateAsync(dto);

		}



		// 取得類別
		public async Task<IEnumerable<SelectListItem>> GetCategorySelectListAsync()
		{
			return await _context.ArticleCategories
				.Where(x => x.IsEnabled) // 撈啟用的
				.OrderBy(x => x.SortOrder)
				.Select(x => new SelectListItem
				{
					Value = x.Id.ToString(),
					Text = x.Name
				}).ToListAsync();
		}

		// 取得活動
		public async Task<IEnumerable<SelectListItem>> GetEventSelectListAsync()
		{
			return await _context.Events
				.Where(x => x.Status == 1 || x.Status == 0) // 取進行中及未開始活動
				.Select(x => new SelectListItem
				{
					Value = x.Id.ToString(),
					Text = x.Title
				}).ToListAsync();
		}

		// 取得首頁列表
		public async Task<List<ArticleDto>> GetAllForIndexAsync()
		{
			return await _repo.GetAllAsync();
			
		}

		// 取得資料
		public async Task<ArticleEditDto> GetByIdAsync(int id)
		{
			var result = await _repo.GetEditByIdAsync(id);
			if (result == null)
			{
				throw new Exception("找不到此文章");

			}
			return result;
		}


	}
}
