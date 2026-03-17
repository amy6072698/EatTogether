using EatTogether.Models.DTOs;
using EatTogether.Models.Infra;
using EatTogether.Models.Repositories;
using System.Linq;

namespace EatTogether.Models.Services
{
    public class ReservationService
    {
        private readonly IReservationRepository _repo;
        private readonly ITableRepository _tableRepo;
        private readonly ReservationEmailService _emailService;

        public ReservationService(IReservationRepository repo, ITableRepository tableRepo, ReservationEmailService emailService)
        {
            _repo = repo;
            _tableRepo = tableRepo;
            _emailService = emailService;
        }

        public async Task<IEnumerable<ReservationDto>> GetAllAsync() => await _repo.GetAllAsync();
        public async Task<IEnumerable<ReservationDto>> GetByDateAsync(DateTime date) => await _repo.GetByDateAsync(date);

        // ── 營業時間設定 ──
        private const int OPEN_HOUR = 11;   // 最早 11:00
        private const int CLOSE_HOUR = 19;   // 最晚 19:xx（含 19:45）
        private const int SESSION_CAPACITY_PERCENT = 70;
        private static readonly int[] ValidMinutes = { 0, 15, 30, 45 };

        /// <summary>取得以 dt 為中心的 ±90 分鐘衝突窗口</summary>
        private static (DateTime start, DateTime end) GetConflictWindow(DateTime dt)
            => (dt.AddMinutes(-90), dt.AddMinutes(90));

        public async Task<Result> CreateAsync(ReservationDto dto)
        {
            var d = dto.ReservationDate;
            int totalPeople = dto.AdultsCount + dto.ChildrenCount;

            // ① 30 分鐘限制
            if (d < DateTime.Now.AddMinutes(30))
                return Result.Fail("訂位時間必須在 30 分鐘後，請重新選擇時間");

            // ② 營業時間 11:00~19:45
            if (d.Hour < OPEN_HOUR || d.Hour > CLOSE_HOUR)
                return Result.Fail($"訂位時間須在 {OPEN_HOUR:D2}:00~{CLOSE_HOUR:D2}:45 之間");

            // ③ 分鐘只能選 00、15、30、45
            if (!ValidMinutes.Contains(d.Minute))
                return Result.Fail("訂位時間分鐘只能選 00、15、30、45");

            // ④ 桌型對應
            var allTables = (await _tableRepo.GetAllAsync()).ToList();
            int requiredSeats = totalPeople <= 2 ? 2
                              : totalPeople <= 4 ? 4
                              : totalPeople <= 6 ? 6 : 10;

            if (totalPeople > 10)
                return Result.Fail("訂位人數上限為 10 人（最大桌型為 10 人桌）");

            // ⑤ ±90 分鐘衝突窗口內，同桌型組數限制
            var (windowStart, windowEnd) = GetConflictWindow(d);
            var windowReservations = (await _repo.GetBySessionAsync(windowStart, windowEnd)).ToList();

            int tableCountOfType = allTables.Count(t => t.SeatCount == requiredSeats);
            if (tableCountOfType == 0)
                return Result.Fail($"目前沒有 {requiredSeats} 人桌");

            int bookedGroupsOfType = windowReservations.Count(r =>
            {
                int people = r.AdultsCount + r.ChildrenCount;
                int seats = people <= 2 ? 2
                           : people <= 4 ? 4
                           : people <= 6 ? 6 : 10;
                return seats == requiredSeats;
            });

            if (bookedGroupsOfType >= tableCountOfType)
                return Result.Fail(
                    $"此時間（{d:HH:mm}）前後 90 分鐘內，{requiredSeats} 人桌已全數預訂" +
                    $"（共 {tableCountOfType} 張，已訂 {bookedGroupsOfType} 組），請選擇其他時間或桌型");

            // ⑥ ±90 分鐘窗口內總人數 70% 容量限制
            int totalCapacity = allTables.Sum(t => t.SeatCount);
            int maxCapacity = (int)(totalCapacity * SESSION_CAPACITY_PERCENT / 100.0);
            int bookedCount = windowReservations.Sum(r => r.AdultsCount + r.ChildrenCount);
            if (bookedCount + totalPeople > maxCapacity)
                return Result.Fail(
                    $"此時間（{d:HH:mm}）前後 90 分鐘內訂位人數已達上限" +
                    $"（{bookedCount}/{maxCapacity}），請選擇其他時間");

            // ⑦ 產生 BookingNumber
            var seq = await _repo.GetMaxSeqOfMonthAsync(d.Year, d.Month) + 1;
            dto.BookingNumber = $"R{d.Year % 100:D2}{d.Month:D2}{seq:D3}";
            await _repo.CreateAsync(dto);

            // ⑧ 寄發訂位確認信（寫入 EmailQueue）
            if (!string.IsNullOrEmpty(dto.Email))
            {
                var body = _emailService.BuildReservationEmail(
                    dto.Name, dto.BookingNumber, dto.ReservationDate,
                    dto.AdultsCount, dto.ChildrenCount, dto.Remark);
                await _emailService.EnqueueAsync(
                    dto.Email,
                    $"【義起吃】訂位確認 - {dto.BookingNumber}",
                    body);
            }

            return Result.Success();
        }

        public async Task<Result> UpdateStatusAsync(int id, int newStatus)
        {
            var r = await _repo.GetByIdAsync(id);
            if (r == null) return Result.Fail("找不到此訂位");
            if (r.Status == 1)
                return Result.Fail("此訂位已完成報到，無法再更改狀態");
            await _repo.UpdateStatusAsync(id, newStatus);
            return Result.Success();
        }
    }
}
