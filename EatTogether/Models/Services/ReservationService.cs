using EatTogether.Models.DTOs;
using EatTogether.Models.Infra;
using EatTogether.Models.Repositories;

namespace EatTogether.Models.Services
{
    public class ReservationService
    {
        private readonly IReservationRepository _repo;
        public ReservationService(IReservationRepository repo) { _repo = repo; }

        public async Task<IEnumerable<ReservationDto>> GetAllAsync() => await _repo.GetAllAsync();
        public async Task<IEnumerable<ReservationDto>> GetByDateAsync(DateTime date) => await _repo.GetByDateAsync(date);

        public async Task<Result> CreateAsync(ReservationDto dto)
        {
            if (await _repo.IsConflictAsync(dto.ReservationDate))
                return Result.Fail("此時段（+/-90 分鐘內）已有訂位，請選擇其他時間");
            var now = dto.ReservationDate;
            var seq = await _repo.GetMaxSeqOfMonthAsync(now.Year, now.Month) + 1;
            dto.BookingNumber = $"R{now.Year % 100:D2}{now.Month:D2}{seq:D3}";
            await _repo.CreateAsync(dto);
            return Result.Success();
        }

        public async Task<Result> UpdateStatusAsync(int id, int newStatus)
        {
            var r = await _repo.GetByIdAsync(id);
            if (r == null) return Result.Fail("找不到此訂位");
            await _repo.UpdateStatusAsync(id, newStatus);
            return Result.Success();
        }
    }
}
