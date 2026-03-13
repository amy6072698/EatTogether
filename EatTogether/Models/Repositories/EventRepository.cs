using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Repositories
{
	public interface IEventRepository
	{
		void Create(EventCreateDto dto);
		List<EventDto> GetAll();
	}

	public class EventRepository : IEventRepository
	{
		private readonly EatTogetherDBContext _context;

		public EventRepository(EatTogetherDBContext context)
		{
			_context = context;
		}
				
		public void Create(EventCreateDto dto)
		{
			var events = dto.ToEntity();

			_context.Events.Add(events);
			_context.SaveChanges();

		}

		public List<EventDto> GetAll()
		{
			var data = _context.Events
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
				.ToList();

			return data;

		}
	}
}
