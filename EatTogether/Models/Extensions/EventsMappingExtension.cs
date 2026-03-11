using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.ViewModels;

namespace EatTogether.Models.Extensions
{
	public static class EventsMappingExtension
	{
		//活動新增
		public static EventCreateDto ToDto(this EventCreateViewModel vm)
		{
			return new EventCreateDto
			{
				Id = vm.Id,
				Title = vm.Title,
				Summary = vm.Summary,
				MinSpend = vm.MinSpend,
				StartDate = vm.StartDate,
				EndDate = vm.EndDate,
				RewardItem = vm.RewardItem,
				DiscountType = vm.DiscountType,
				DiscountValue = vm.DiscountValue,
				Status = vm.Status
			};
		}

		public static Event ToEntity(this EventCreateDto dto)
		{
			return new Event
			{
				Id = dto.Id,
				Title = dto.Title,
				Summary = dto.Summary,
				MinSpend = dto.MinSpend,
				StartDate = dto.StartDate,
				EndDate = dto.EndDate,
				RewardItem = dto.RewardItem,
				DiscountType = dto.DiscountType,
				DiscountValue = dto.DiscountValue,
				Status = dto.Status
			};
		}


		//活動編輯
		//	vm<-> dto
 		//→ ToDto(this EventEditViewModel vm)
		//→ ToViewModel(this AEventEditDto dto)
		//	// Dto → Entity（Repository 寫入用）
		//	→Event ToEntity(this EventEditDto dto)

		//	// Entity → Dto（Repository 讀取用）
		//	EventDto ToDto(this Event entity)



	}
}
