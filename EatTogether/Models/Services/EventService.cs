using EatTogether.Models.DTOs;
using EatTogether.Models.Repositories;
using EatTogether.Models.ViewModels;

namespace EatTogether.Models.Services
{
	public class EventService
	{
		private readonly IEventRepository _repo;

		public EventService(IEventRepository repo)
		{
			_repo = repo;
		}

		// 新增活動
		public async Task<EventServiceResult<bool>> CreateAsync(EventCreateDto dto)
		{
			try
			{
				await _repo.CreateAsync(dto);
				return EventServiceResult<bool>.Ok(true);
				//{
				//	Success = true,
				//	Data = true,
				//	Message = "新增活動完成！"
				//};
			}
			catch (Exception ex)
			{
				return EventServiceResult<bool>.Fail($"新增失敗：{ex.Message}");
			}
		}

		// 取得首頁列表
		public async Task<List<EventDto>> GetAllForIndexAsync()
		{
			return await _repo.GetAllAsync();
		}

		// 取得編輯用的資料
		public async Task<EventEditDto> GetEditByIdAsync(int id)
		{
			var result = await _repo.GetEditByIdAsync(id);
			if (result == null)
			{
				throw new Exception("找不到此活動");

			}
			return result;
		}

		// 編輯活動
		public async Task<EventServiceResult<bool>> EditAsync(EventEditDto dto)
		{
			try
			{
				await _repo.EditAsync(dto);

				return EventServiceResult<bool>.Ok(true);
				//{
				//	Success = true,
				//	Data = true,
				//	Message = "編輯活動完成！"
				//};
			}
			catch (Exception ex)
			{
				return EventServiceResult<bool>.Fail($"編輯失敗：{ex.Message}");
			}
		}

		//停用活動
		public async Task DeactivateAsync(int id)
		{
			var ev = await GetEditByIdAsync(id);
			ev.Status = 2;
			await _repo.EditAsync(ev);
		}

		//複製為新活動
		public async Task<EventCreateViewModel> GetCopyCreateVm(int id)
		{
			var source = await GetEditByIdAsync(id);
			if (source == null) return null;

			return new EventCreateViewModel
			{
				Title = source.Title,
				Summary = source.Summary,
				MinSpend = source.MinSpend,
				RewardItem = source.RewardItem,
				DiscountType = source.DiscountType,
				DiscountValue = source.DiscountValue,
				StartDate = DateTime.Today,
				EndDate = DateTime.Today
				//,Status = 0
			};
		}


	}
}
