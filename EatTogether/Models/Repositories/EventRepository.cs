using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Repositories
{

	public class EventRepository : IEventRepository
	{
		private readonly EatTogetherDBContext _context;

		public EventRepository(EatTogetherDBContext context)
		{
			_context = context;
		}

		public async Task CreateAsync(EventCreateDto dto)
		{
			var events = dto.ToEntity();

			_context.Events.Add(events);
			await _context.SaveChangesAsync();
		}


		public async Task<List<EventDto>> GetAllAsync()
		{
			var data = await _context.Events
				.AsNoTracking()
				.Select(e => new EventDto
				{
					Id = e.Id,
					Title = e.Title,
					Summary = e.Summary,
					MinSpend = e.MinSpend,
					StartDate = e.StartDate,
					EndDate = e.EndDate,
					RewardDishId = e.RewardDishId,
					RewardDishName = e.RewardDish != null ? e.RewardDish.DishName : null,
					DiscountType = e.DiscountType,
					DiscountValue = e.DiscountValue,
					Status = e.Status
				})
				.ToListAsync();

			return data;
		}


		public async Task EditAsync(EventEditDto dto)
		{
			var entity = await _context.Events.FindAsync(dto.Id);
			if (entity == null)
			{
				return;
			}

			entity.Title = dto.Title;
			entity.Summary = dto.Summary;
			entity.MinSpend = dto.MinSpend;
			entity.StartDate = dto.StartDate;
			entity.EndDate = dto.EndDate;
			entity.RewardDishId = dto.RewardDishId;
			entity.DiscountType = dto.DiscountType;
			entity.DiscountValue = dto.DiscountValue;
			entity.Status = dto.Status;

			await _context.SaveChangesAsync();		
		}

		public async Task<EventEditDto> GetEditByIdAsync(int id)
		{
			var entity = await _context.Events.FindAsync(id);

			if (entity == null) return null;

			return new EventEditDto
			{
				Id = entity.Id,
				Title = entity.Title,
				Summary = entity.Summary,
				MinSpend = entity.MinSpend,
				StartDate = entity.StartDate,
				EndDate = entity.EndDate,
				RewardDishId = entity.RewardDishId,				
				DiscountType = entity.DiscountType,
				DiscountValue = entity.DiscountValue,
				Status = entity.Status
			};
		}

        public async Task<List<EventApplicableDto>> GetApplicableEventsAsync(int amount)
        {
            var today    = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var events = await _context.Events
                .AsNoTracking()
                .Include(e => e.RewardDish)
                .Where(e => e.Status == 1
                         && e.IsAutoDiscount == 1
                         && e.StartDate < tomorrow
                         && e.EndDate   >= today
                         && e.MinSpend  <= amount)
                .OrderByDescending(e => e.MinSpend)
                .ToListAsync();

            var result = new List<EventApplicableDto>();

            foreach (var e in events)
            {
                int calculated = 0;
                string desc    = string.Empty;
                var dishName   = e.RewardDish?.DishName ?? "";

                if (e.DiscountType == "FixedAmount")
                {
                    calculated = (int)e.DiscountValue;
                    desc = $"折抵 NT${calculated}";
                }
                else if (e.DiscountType == "Percent")
                {
                    calculated = (int)Math.Round(amount * (1 - (double)e.DiscountValue / 10));
                    desc = $"打 {e.DiscountValue} 折，省 NT${calculated}";
                }
                else
                {
                    desc = $"贈送：{dishName}";
                }

                result.Add(new EventApplicableDto
                {
                    Id                  = e.Id,
                    Title               = e.Title,
                    Summary             = e.Summary ?? string.Empty,
                    DiscountType        = e.DiscountType,
                    DiscountValue       = e.DiscountValue,
                    RewardDishId        = e.RewardDishId,
                    RewardDishName      = string.IsNullOrEmpty(dishName) ? null : dishName,
                    MinSpend            = e.MinSpend,
                    CalculatedDiscount  = calculated,
                    DiscountDescription = desc
                });
            }

            return result;
        }
    }
}
