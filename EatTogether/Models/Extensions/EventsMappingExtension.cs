using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.ViewModels;

namespace EatTogether.Models.Extensions
{
	public static class EventsMappingExtension
	{
		//活動新增
		public static EventCreateDto ToCreateDto(this EventCreateViewModel vm)
		{
			return new EventCreateDto
			{
				//Id = vm.Id,
				Title = vm.Title,
				Summary = vm.Summary,
				MinSpend = vm.MinSpend,
				StartDate = vm.StartDate,
				EndDate = vm.EndDate,
				RewardItem = vm.RewardItem,
				DiscountType = vm.DiscountType,
				DiscountValue = vm.DiscountValue,
				Status = CalculateStatus(vm.StartDate, vm.EndDate)
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
				Status = CalculateStatus(dto.StartDate, dto.EndDate)
			};
		}

		// 自動計算狀態的方法
		private static int CalculateStatus(DateTime startDate, DateTime endDate)
		{
			var today = DateTime.Today;

			if (today < startDate)
				return 0; // 未開始
			else if (today >= startDate && today <= endDate)
				return 1; // 進行中
			else
				return 2; // 已結束
		}



		//活動列表 dto -> vm
		//→ ToViewModel(this EventDto dto)
		//// Entity → Dto（Repository 讀取用）
		//EventDto ToDto(this Event entity)

		public static EventViewModel ToEventVm(this EventDto dto)
		{
			return new EventViewModel
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

		public static EventDto ToEventDto(this Event entity)
		{
			return new EventDto
			{
				Id = entity.Id,
				Title = entity.Title,
				Summary = entity.Summary,
				MinSpend = entity.MinSpend,
				StartDate = entity.StartDate,
				EndDate = entity.EndDate,
				RewardItem = entity.RewardItem,
				DiscountType = entity.DiscountType,
				DiscountValue = entity.DiscountValue,
				Status = entity.Status
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

		public static EventEditDto ToEditDto(this EventEditViewModel vm)
		{
			return new EventEditDto
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
				Status = CalculateStatus(vm.StartDate, vm.EndDate)
			};
		}

		public static EventEditViewModel ToEditVm(this EventEditDto dto)
		{
			return new EventEditViewModel
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

		public static Event ToEntity(this EventEditDto dto)
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
				Status = CalculateStatus(dto.StartDate, dto.EndDate)
			};
		}

		public static EventEditDto ToEditDto(this Event entity)
		{
			return new EventEditDto
			{
				Id = entity.Id,
				Title = entity.Title,
				Summary = entity.Summary,
				MinSpend = entity.MinSpend,
				StartDate = entity.StartDate,
				EndDate = entity.EndDate,
				RewardItem = entity.RewardItem,
				DiscountType = entity.DiscountType,
				DiscountValue = entity.DiscountValue,
				Status = entity.Status
			};
		}


	}
}
