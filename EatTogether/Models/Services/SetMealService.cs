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
		}

		public async Task DisableAsync(int id)
		{
			await _repo.SoftDeleteAsync(id);
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
	}
}
