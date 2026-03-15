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
        public ReservationService(IReservationRepository repo, ITableRepository tableRepo)
        {
            _repo = repo;
            _tableRepo = tableRepo;
        }

        public async Task<IEnumerable<ReservationDto>> GetAllAsync() => await _repo.GetAllAsync();
        public async Task<IEnumerable<ReservationDto>> GetByDateAsync(DateTime date) => await _repo.GetByDateAsync(date);

        // ── 時段設定（每天 11~20:00，每 2hr 一時段）──
        private static readonly int[] ValidHours = { 11, 13, 15, 17, 19, 20 };
        private const int SESSION_CAPACITY_PERCENT = 70;

        private static (DateTime start, DateTime end) GetSessionRange(DateTime dt)
        {
            int h = dt.Hour;
            int sessionStart = ValidHours.Where(x => x <= h).DefaultIfEmpty(11).Max();
            return (dt.Date.AddHours(sessionStart), dt.Date.AddHours(sessionStart + 2));
        }

        public async Task<Result> CreateAsync(ReservationDto dto)
        {
            var d = dto.ReservationDate;
            int totalPeople = dto.AdultsCount + dto.ChildrenCount;

            // ① 30 分鐘限制
            if (d < DateTime.Now.AddMinutes(30))
                return Result.Fail("訂位時間必須在 30 分鐘後，請重新選擇時間");

            // ② 營業時間 11:00~20:00
            if (d.Hour < 11 || d.Hour > 20 || (d.Hour == 20 && d.Minute > 0))
                return Result.Fail("訂位時間須在 11:00~20:00 之間");

            // ③ 必須為合法整點時段
            if (!ValidHours.Contains(d.Hour) || d.Minute != 0)
            {
                var validStr = string.Join("、", ValidHours.Select(h => $"{h:D2}:00"));
                return Result.Fail($"訂位時間必須選擇整點時段：{validStr}");
            }

            // ④ 桌型對應
            var allTables = (await _tableRepo.GetAllAsync()).ToList();
            int requiredSeats = totalPeople <= 2 ? 2
                              : totalPeople <= 4 ? 4
                              : totalPeople <= 6 ? 6 : 10;

            // 超過最大桌型（10人）直接拒絕
            if (totalPeople > 10)
                return Result.Fail("訂位人數上限為 10 人（最大桌型為 10 人桌）");

            // ⑤ 同時段桌型組數限制
            //    同一時段該桌型已訂組數 不可超過 該桌型的桌子總數
            var (sessionStart, sessionEnd) = GetSessionRange(d);
            var sessionReservations = (await _repo.GetBySessionAsync(sessionStart, sessionEnd)).ToList();

            // 計算各桌型對應的桌子數量
            int tableCountOfType = allTables.Count(t => t.SeatCount == requiredSeats);
            if (tableCountOfType == 0)
                return Result.Fail($"目前沒有 {requiredSeats} 人桌");

            // 計算同時段已訂同桌型的組數
            int bookedGroupsOfType = sessionReservations.Count(r =>
            {
                int people = r.AdultsCount + r.ChildrenCount;
                int seats = people <= 2 ? 2
                           : people <= 4 ? 4
                           : people <= 6 ? 6 : 10;
                return seats == requiredSeats;
            });

            if (bookedGroupsOfType >= tableCountOfType)
                return Result.Fail(
                    $"此時段（{sessionStart:HH:mm}~{sessionEnd:HH:mm}）{requiredSeats} 人桌已全數預訂" +
                    $"（共 {tableCountOfType} 張，已訂 {bookedGroupsOfType} 組），請選擇其他時段或桌型");

            // ⑥ 同時段總人數 70% 容量限制
            int totalCapacity = allTables.Sum(t => t.SeatCount);
            int maxCapacity = (int)(totalCapacity * SESSION_CAPACITY_PERCENT / 100.0);
            int bookedCount = sessionReservations.Sum(r => r.AdultsCount + r.ChildrenCount);
            if (bookedCount + totalPeople > maxCapacity)
                return Result.Fail(
                    $"此時段（{sessionStart:HH:mm}~{sessionEnd:HH:mm}）訂位人數已達上限" +
                    $"（{bookedCount}/{maxCapacity}），請選擇其他時段");

            // ⑦ 產生 BookingNumber
            var seq = await _repo.GetMaxSeqOfMonthAsync(d.Year, d.Month) + 1;
            dto.BookingNumber = $"R{d.Year % 100:D2}{d.Month:D2}{seq:D3}";
            await _repo.CreateAsync(dto);
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
