using EatTogether.Models.DTOs;
using EatTogether.Models.Repositories;

namespace EatTogether.Models.Services
{
	public class SetMealService
	{
		private readonly ISetMealRepository _repo;

		public SetMealService(ISetMealRepository repo)
		{
			_repo = repo;
		}

		public async Task <IEnumerable<Setmealdto>> GetAllAsync()
		{
			return await _repo.GetAllAsync();
		}

		public async Task <IEnumerable<Setmealdto>> GetAllActiveAsync()
		{
			return await _repo.GetAllActiveAsync();
		}

		public async Task<Setmealdto?> GetByIdAsync(int id)
		{
			return await _repo.GetByIdAsync(id);
		}

		public async Task CreateAsync(Setmealdto dto)
		{
			await _repo.CreateAsync(dto);
		}
		public async Task UpdateAsync(Setmealdto dto)
		{
			await _repo.UpdateAsync(dto);
			if (dto.Items != null)
			{
				await _repo.UpdateItemsAsync(dto.Id, dto.Items);
			}
		}

		public async Task DisableAsync(int id)
		{
			await _repo.SoftDeleteAsync(id);
		}

		public async Task BatchDisableAsync(IEnumerable<int> ids)
		{
			await _repo.BatchSoftDeleteAsync(ids);
		}

		public async Task EnableAsync(int id)
		{
			await _repo.EnableAsync(id);
		}

		public async Task BatchEnableAsync(IEnumerable<int> ids)
		{
			await _repo.BatchEnableAsync(ids);
		}

		public async Task DeleteAsync(int id)
		{
			await _repo.DeleteAsync(id);
		}

		public async Task BatchDeleteAsync(IEnumerable<int> ids)
		{
			await _repo.BatchDeleteAsync(ids);
		}

		public async Task AddItemAsync(SetmealItemDto itemDto)
		{
			// 驗證互斥邏輯：IsOptional=true 時 OptionGroupNo 和 PickLimit 必填
			if (itemDto.IsOptional &&
			   (!itemDto.OptionGroupNo.HasValue || !itemDto.PickLimit.HasValue))
			{
				throw new InvalidOperationException("選擇性項目必須填寫選項群組編號和選取上限");
			}

			// 驗證互斥邏輯：IsOptional=false 時 OptionGroupNo 和 PickLimit 應為 null
			if (!itemDto.IsOptional)
			{
				itemDto.OptionGroupNo = null;
				itemDto.PickLimit = null;
			}

			await _repo.AddItemAsync(itemDto);
		}

		public async Task RemoveItemAsync(int itemId)
		{
			await _repo.RemoveItemAsync(itemId);
		}

		public async Task UpdateItemsAsync(int setMealId, IEnumerable<SetmealItemDto> itemDtos)
		{
			// 在這裡可以加入服務層的驗證邏輯，例如檢查總價等，此處暫略
			await _repo.UpdateItemsAsync(setMealId, itemDtos);
		}

		public async Task UpdateOrderAsync(IEnumerable<int> orderedIds)
		{
			await _repo.UpdateOrderAsync(orderedIds);
		}
	}
}
