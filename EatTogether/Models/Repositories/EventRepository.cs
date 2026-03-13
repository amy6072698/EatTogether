using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Repositories
{
	public interface IEventRepository
	{
		Task CreateAsync(EventCreateDto dto);
		Task<List<EventDto>> GetAllAsync();
		Task EditAsync(EventEditDto dto);
		EventEditDto GetEditById(int id);
	}

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
					RewardItem = e.RewardItem,
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
			entity.RewardItem = dto.RewardItem;
			entity.DiscountType = dto.DiscountType;
			entity.DiscountValue = dto.DiscountValue;
			entity.Status = dto.Status;

			await _context.SaveChangesAsync();		
		}

		public async Task<EventEditDto> GetEditById(int id)
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
				RewardItem = entity.RewardItem,
				DiscountType = entity.DiscountType,
				DiscountValue = entity.DiscountValue,
				Status = entity.Status
			};
		}


	}
}
