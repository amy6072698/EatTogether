using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.Extensions;
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
                                ImageUrl = dto.ImageUrl,
                                StartDate = dto.StartDate,
                                EndDate = dto.EndDate,
                                StartTime = dto.StartTime,
                                EndTime = dto.EndTime,
                                DisplayOrder = dto.DisplayOrder,
                                IsPopular = dto.IsPopular,
                                IsRecommended = dto.IsRecommended
                        };

                        _context.SetMeals.Add(setMeal);
                        await _context.SaveChangesAsync();
                }

                public async Task<IEnumerable<Setmealdto>> GetAllAsync()
                {
                        return await _context.SetMeals
                                .Include(s => s.SetMealItems)
                                .ThenInclude(i => i.Dish)
                                .ThenInclude(d => d.Category)
                                .Select(s => s.ToDo())
                                .ToListAsync();
                }

                public async Task<IEnumerable<Setmealdto>> GetAllActiveAsync()
                {
                        return await _context.SetMeals
                                .Where(s => s.IsActive)
                                .Include(s => s.SetMealItems)
                                .ThenInclude(i => i.Dish)
                                .ThenInclude(d => d.Category)
                                .Select(s => s.ToDo())
                                .ToListAsync();
                }

                public async Task<Setmealdto?> GetByIdAsync(int id)
                {
                    var setMeal = await _context.SetMeals
                                    .Where(s => s.Id == id)
                                    .Include(s => s.SetMealItems)
                                    .ThenInclude(i => i.Dish)
                                    .ThenInclude(d => d.Category)
                                    .FirstOrDefaultAsync();

                    return setMeal?.ToDo();
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

                public async Task BatchSoftDeleteAsync(IEnumerable<int> ids)
                {
                        var setMeals = await _context.SetMeals.Where(s => ids.Contains(s.Id)).ToListAsync();
                        foreach (var s in setMeals)
                        {
                                s.IsActive = false;
                                s.UpdatedAt = DateTime.Now;
                        }
                        await _context.SaveChangesAsync();
                }

                public async Task EnableAsync(int id)
                {
                        var setMeal = await _context.SetMeals.FindAsync(id);
                        if (setMeal == null) return;

                        setMeal.IsActive = true;
                        setMeal.UpdatedAt = DateTime.Now;

                        await _context.SaveChangesAsync();
                }

                public async Task BatchEnableAsync(IEnumerable<int> ids)
                {
                        var setMeals = await _context.SetMeals.Where(s => ids.Contains(s.Id)).ToListAsync();
                        foreach (var s in setMeals)
                        {
                                s.IsActive = true;
                                s.UpdatedAt = DateTime.Now;
                        }
                        await _context.SaveChangesAsync();
                }

                public async Task DeleteAsync(int id)
                {
                        var setMeal = await _context.SetMeals
                                .Include(s => s.SetMealItems)
                                .Include(s => s.Products)
                                .FirstOrDefaultAsync(s => s.Id == id);
                        if (setMeal == null) return;

                        // 1. 刪除相關的 SetMealItems
                        if (setMeal.SetMealItems.Any())
                        {
                                _context.SetMealItems.RemoveRange(setMeal.SetMealItems);
                        }

                        // 2. 處理相關的 Products (將 SetMealId 設為 null)
                        foreach (var p in setMeal.Products)
                        {
                                p.SetMealId = null;
                        }

                        // 3. 刪除 SetMeal
                        _context.SetMeals.Remove(setMeal);
                        await _context.SaveChangesAsync();
                }

                public async Task BatchDeleteAsync(IEnumerable<int> ids)
                {
                        // 1. 刪除相關的 SetMealItems
                        var items = await _context.SetMealItems.Where(i => ids.Contains(i.SetMealId)).ToListAsync();
                        if (items.Any())
                        {
                                _context.SetMealItems.RemoveRange(items);
                                await _context.SaveChangesAsync();
                        }

                        // 2. 處理相關的 Products (將 SetMealId 設為 null)
                        var products = await _context.Products.Where(p => p.SetMealId.HasValue && ids.Contains(p.SetMealId.Value)).ToListAsync();
                        foreach (var p in products)
                        {
                                p.SetMealId = null;
                        }
                        await _context.SaveChangesAsync();

                        // 3. 刪除 SetMeals
                        var setMeals = await _context.SetMeals.Where(s => ids.Contains(s.Id)).ToListAsync();
                        _context.SetMeals.RemoveRange(setMeals);
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
                        setMeal.StartDate = dto.StartDate;
                        setMeal.EndDate = dto.EndDate;
                        setMeal.StartTime = dto.StartTime;
                        setMeal.EndTime = dto.EndTime;
                        setMeal.DisplayOrder = dto.DisplayOrder;
                        setMeal.IsPopular = dto.IsPopular;
                        setMeal.IsRecommended = dto.IsRecommended;
                        setMeal.UpdatedAt = DateTime.Now;

                        await _context.SaveChangesAsync();
                }

                public async Task UpdateItemsAsync(int setMealId, IEnumerable<SetmealItemDto> itemDtos)
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        // 1. 刪除該套餐現有的所有項目
                        var existingItems = await _context.SetMealItems
                                                .Where(i => i.SetMealId == setMealId)
                                                .ToListAsync();
                        
                        if (existingItems.Any())
                        {
                            _context.SetMealItems.RemoveRange(existingItems);
                            await _context.SaveChangesAsync();
                        }

                        // 2. 新增傳入的項目
                        if (itemDtos != null && itemDtos.Any())
                        {
                            var newItems = itemDtos.Select(dto => new SetMealItem
                            {
                                SetMealId = setMealId,
                                DishId = dto.DishId,
                                Quantity = dto.Quantity,
                                IsOptional = dto.IsOptional,
                                OptionGroupNo = dto.IsOptional ? dto.OptionGroupNo : null,
                                PickLimit = dto.IsOptional ? dto.PickLimit : null,
                                DisplayOrder = dto.DisplayOrder
                            }).ToList();

                            await _context.SetMealItems.AddRangeAsync(newItems);
                            await _context.SaveChangesAsync();
                        }

                        // 3. 提交事務
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new Exception("更新套餐餐點失敗: " + ex.Message);
                    }
                }

                public async Task UpdateOrderAsync(IEnumerable<int> orderedIds)
                {
                    var setMealsToUpdate = await _context.SetMeals
                                                         .Where(s => orderedIds.Contains(s.Id))
                                                         .ToListAsync();

                    var idToOrderMap = orderedIds
                        .Select((id, index) => new { Id = id, Order = index + 1 })
                        .ToDictionary(x => x.Id, x => x.Order);

                    foreach (var setMeal in setMealsToUpdate)
                    {
                        if (idToOrderMap.TryGetValue(setMeal.Id, out var order))
                        {
                            setMeal.DisplayOrder = order;
                        }
                    }

                    await _context.SaveChangesAsync();
                }
        }
}
