using EatTogether.Models.DTOs;
using EatTogether.Models.Repositories;

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
				// 在 Ok() 中加入成功的文字訊息
				return new EventServiceResult<bool>
				{
					Success = true,
					Data = true,
					Message = "新增活動完成！"
				};
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
				// 即使 Repo 是 void，若執行過程出錯（如資料庫連不上），會跳入 catch
				await _repo.EditAsync(dto);

				return new EventServiceResult<bool>
				{
					Success = true,
					Data = true,
					Message = "編輯活動完成！"
				};
			}
			catch (Exception ex)
			{
				return EventServiceResult<bool>.Fail($"編輯失敗：{ex.Message}");
			}
		}
	}
}
