using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EatTogether.Models.Repositories
{
	public class SetMealRepository : ISetMealRepository
	{
		private readonly EatTogetherDBContext _context;

		public SetMealRepository(EatTogetherDBContext context)
		{
			_context = context;
		}
		public async Task AddItemAsync(SetmealItemDto itemDto)
		{
			var item = new SetMealItem
			{
				SetMealId = itemDto.SetMealId,
				DishId = itemDto.DishId,
				Quantity = itemDto.Quantity,
				IsOptional = itemDto.IsOptional,
				OptionGroupNo = itemDto.OptionGroupNo,
				PickLimit = itemDto.PickLimit,
				DisplayOrder = itemDto.DisplayOrder
			};

			_context.SetMealItems.Add(item);
			await _context.SaveChangesAsync();
		}

		public async Task CreateAsync(Setmealdto dto)
		{
			var setMeal = new SetMeal
			{
				SetMealName = dto.SetMealName,
				DiscountType = dto.DiscountType,
				DiscountValue = dto.DiscountValue,
				IsActive = true,
				CreatedAt = DateTime.Now,
				SetPrice = dto.SetPrice,
				Description = dto.Description,
				ImageUrl = dto.ImageUrl
			};

			_context.SetMeals.Add(setMeal);
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<Setmealdto>> GetAllAsync()
		{
			return await _context.SetMeals
				.Where(s => s.IsActive)
				.Include(s => s.SetMealItems)
					.ThenInclude(i => i.Dish)
				.Select(s => new Setmealdto
				{
					Id = s.Id,
					SetMealName = s.SetMealName,
					DiscountType = s.DiscountType,
					DiscountValue = s.DiscountValue,
					IsActive = s.IsActive,
					CreatedAt = s.CreatedAt,
					SetPrice = s.SetPrice,
					Description = s.Description,
					ImageUrl = s.ImageUrl,
					UpdatedAt = s.UpdatedAt,
					Items = s.SetMealItems.Select(i => new SetmealItemDto
					{
						Id = i.Id,
						SetMealId = i.SetMealId,
						DishId = i.DishId,
						DishName = i.Dish != null ? i.Dish.DishName : null,
						DishPrice = i.Dish != null ? i.Dish.Price : null,
						Quantity = i.Quantity,
						IsOptional = i.IsOptional,
						OptionGroupNo = i.OptionGroupNo,
						PickLimit = i.PickLimit,
						DisplayOrder = i.DisplayOrder
					}).ToList()
				})
				.ToListAsync();
		}

		public async Task<Setmealdto?> GetByIdAsync(int id)
		{
			return await _context.SetMeals
					.Where(s => s.Id == id && s.IsActive)
					.Include(s => s.SetMealItems)
						.ThenInclude(i => i.Dish)
					.Select(s => new Setmealdto
					{
						Id = s.Id,
						SetMealName = s.SetMealName,
						DiscountType = s.DiscountType,
						DiscountValue = s.DiscountValue,
						IsActive = s.IsActive,
						CreatedAt = s.CreatedAt,
						SetPrice = s.SetPrice,
						Description = s.Description,
						ImageUrl = s.ImageUrl,
						UpdatedAt = s.UpdatedAt,
						Items = s.SetMealItems.Select(i => new SetmealItemDto
						{
							Id = i.Id,
							SetMealId = i.SetMealId,
							DishId = i.DishId,
							DishName = i.Dish != null ? i.Dish.DishName : null,
							DishPrice = i.Dish != null ? i.Dish.Price : null,
							Quantity = i.Quantity,
							IsOptional = i.IsOptional,
							OptionGroupNo = i.OptionGroupNo,
							PickLimit = i.PickLimit,
							DisplayOrder = i.DisplayOrder
						}).ToList()
					})
					.FirstOrDefaultAsync();
		}

		public async Task RemoveItemAsync(int itemId)
		{
			var item = await _context.SetMealItems.FindAsync(itemId);
			if (item == null) return;

			_context.SetMealItems.Remove(item);
			await _context.SaveChangesAsync();
		}
		public async Task SoftDeleteAsync(int id)
		{
			var setMeal = await _context.SetMeals.FindAsync(id);
			if (setMeal == null) return;
			
			setMeal.IsActive = false;
			setMeal.UpdatedAt = DateTime.Now;

			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(Setmealdto dto)
		{
			var setMeal = await _context.SetMeals.FindAsync(dto.Id);
			if (setMeal == null) return;
			
			setMeal.SetMealName = dto.SetMealName;
			setMeal.DiscountType = dto.DiscountType;
			setMeal.DiscountValue = dto.DiscountValue;
			setMeal.SetPrice = dto.SetPrice;
			setMeal.Description = dto.Description;
			setMeal.ImageUrl = dto.ImageUrl;
			setMeal.UpdatedAt = DateTime.Now;

			await _context.SaveChangesAsync();
		}
	}
}
