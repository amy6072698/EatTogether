using EatTogether.Models.DTOs;

namespace EatTogether.Models.Repositories
{
    public interface IReservationRepository
    {
        Task<IEnumerable<ReservationDto>> GetAllAsync();
        Task<IEnumerable<ReservationDto>> GetByDateAsync(DateTime date);
        Task<ReservationDto?> GetByIdAsync(int id);
        Task CreateAsync(ReservationDto dto);
        Task UpdateStatusAsync(int id, int newStatus);
        Task<bool> IsConflictAsync(DateTime reservationDate, int excludeId = 0);
        Task<int> GetMaxSeqOfMonthAsync(int year, int month);
        Task<int> GetSessionBookedCountAsync(DateTime sessionStart, DateTime sessionEnd);
        Task<IEnumerable<ReservationDto>> GetBySessionAsync(DateTime sessionStart, DateTime sessionEnd);
    }
}
