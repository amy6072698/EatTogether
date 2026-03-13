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



		public async Task<bool> CreateAsync(EventCreateDto dto)
		{
			await Task.Run(() => _repo.CreateAsync(dto));
			Console.WriteLine("新增完成");  //考慮新增一個result services
			return true;
		}

		public async Task<List<EventDto>> GetAllForIndexAsync()
		{
			return await _repo.GetAllAsync();
		}

		public async Task<bool> EditAsync(EventEditDto dto)
		{
			await Task.Run(() => _repo.EditAsync(dto));
			Console.WriteLine("編輯完成");  //考慮新增一個result services
			return true;
		}



	}
}
