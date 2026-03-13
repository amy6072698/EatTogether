using EatTogether.Models.DTOs;
using EatTogether.Models.Repositories;

namespace EatTogether.Models.Services
{
	public class EventService
	{
		private readonly IEventRepository _repo;

		public EventService(IEventRepository repo)
		{
			_repo = repo;
		}	



		public bool Create(EventCreateDto dto)
		{
			_repo.Create(dto);
			Console.WriteLine("新增完成");  //考慮新增一個result services
			return true;
		}

		public List<EventDto> GetAllForIndex()
		{
			return _repo.GetAll();
		}



	}
}
