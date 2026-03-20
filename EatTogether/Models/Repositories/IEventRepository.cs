using EatTogether.Models.DTOs;

namespace EatTogether.Models.Repositories
{
	public interface IEventRepository
	{
		Task CreateAsync(EventCreateDto dto);
		Task<List<EventDto>> GetAllAsync();
		Task EditAsync(EventEditDto dto);
		Task<EventEditDto> GetEditByIdAsync(int id);
		Task<List<EventApplicableDto>> GetApplicableEventsAsync(int amount);
		Task<List<EventApplicableDto>> GetManualEventsAsync(int amount);
        /// <summary>回傳活動類型與贈品名稱（含 RewardDish Join），用於結帳頁手動套用 Gift 活動時建立贈品明細。</summary>
        Task<(string DiscountType, string? RewardDishName)?> GetEventGiftInfoAsync(int eventId);
        /// <summary>依 ID 清單取回活動 DTO（不限門檻），用於顯示使用中但已不符目前金額門檻的活動。</summary>
        Task<List<EventApplicableDto>> GetEventsByIdsAsync(IEnumerable<int> ids);
	}
}
