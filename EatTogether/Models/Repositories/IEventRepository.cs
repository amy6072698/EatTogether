using EatTogether.Models.DTOs;

namespace EatTogether.Models.Repositories
{
	public interface IEventRepository
	{
		Task CreateAsync(EventCreateDto dto);
		Task<List<EventDto>> GetAllAsync();
		Task EditAsync(EventEditDto dto);
		Task<EventEditDto> GetEditByIdAsync(int id);
	}
}
