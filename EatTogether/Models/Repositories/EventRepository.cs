using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.Extensions;

namespace EatTogether.Models.Repositories
{
	public interface IEventRepository
	{
		void Create(EventCreateDto dto);
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
	}
}
