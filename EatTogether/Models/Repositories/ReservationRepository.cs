using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly EatTogetherDBContext _context;

        public ReservationRepository(EatTogetherDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReservationDto>> GetAllAsync()
        {
            return await _context.Reservations
                .OrderByDescending(r => r.ReservationDate)
                .Select(r => r.ToDto())
                .ToListAsync();
        }

        public async Task<IEnumerable<ReservationDto>> GetByDateAsync(DateTime date)
        {
            return await _context.Reservations
                .Where(r => r.ReservationDate.Date == date.Date)
                .OrderBy(r => r.ReservationDate)
                .Select(r => r.ToDto())
                .ToListAsync();
        }

        public async Task<ReservationDto?> GetByIdAsync(int id)
        {
            var r = await _context.Reservations.FindAsync(id);
            return r?.ToDto();
        }

        public async Task CreateAsync(ReservationDto dto)
        {
            var reservation = new Reservation
            {
                BookingNumber = dto.BookingNumber,
                Name = dto.Name,
                Phone = dto.Phone,
                Email = dto.Email,
                ReservationDate = dto.ReservationDate,
                AdultsCount = dto.AdultsCount,
                ChildrenCount = dto.ChildrenCount,
                Status = 0,    // 新增預設為「訂位中」
                Remark = dto.Remark,
                ReservedAt = DateTime.Now
            };
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, int newStatus)
        {
            var r = await _context.Reservations.FindAsync(id);
            if (r == null) return;

            r.Status = newStatus;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 檢查 ±90 分鐘內是否有 Status IN (0,1) 的衝突訂位
        /// </summary>
        public async Task<bool> IsConflictAsync(DateTime reservationDate, int excludeId = 0)
        {
            var windowStart = reservationDate.AddMinutes(-90);
            var windowEnd = reservationDate.AddMinutes(90);

            return await _context.Reservations
                .Where(r => r.Id != excludeId
                         && (r.Status == 0 || r.Status == 1)
                         && r.ReservationDate >= windowStart
                         && r.ReservationDate <= windowEnd)
                .AnyAsync();
        }

        /// <summary>
        /// 取得該年月最大序號（產生 BookingNumber 用）
        /// </summary>
        public async Task<int> GetMaxSeqOfMonthAsync(int year, int month)
        {
            // BookingNumber 格式：R + 年後2碼 + 月2碼 + 序號3碼
            // e.g. R260311006
            var prefix = $"R{year % 100:D2}{month:D2}";

            var last = await _context.Reservations
                .Where(r => r.BookingNumber.StartsWith(prefix))
                .OrderByDescending(r => r.BookingNumber)
                .Select(r => r.BookingNumber)
                .FirstOrDefaultAsync();

            if (last == null) return 0;

            // 取最後 3 碼序號
            if (int.TryParse(last.Substring(prefix.Length), out int seq))
                return seq;

            return 0;
        }
        /// <summary>取得某時段（sessionStart ~ sessionEnd）的已訂人數（Status 0/1）</summary>
        public async Task<int> GetSessionBookedCountAsync(DateTime sessionStart, DateTime sessionEnd)
        {
            return await _context.Reservations
                .Where(r => (r.Status == 0 || r.Status == 1)
                         && r.ReservationDate >= sessionStart
                         && r.ReservationDate < sessionEnd)
                .SumAsync(r => (int?)(r.AdultsCount + r.ChildrenCount)) ?? 0;
        }

        /// <summary>取得某時段內所有有效訂位</summary>
        public async Task<IEnumerable<ReservationDto>> GetBySessionAsync(DateTime sessionStart, DateTime sessionEnd)
        {
            return await _context.Reservations
                .Where(r => (r.Status == 0 || r.Status == 1)
                         && r.ReservationDate >= sessionStart
                         && r.ReservationDate < sessionEnd)
                .OrderBy(r => r.ReservationDate)
                .Select(r => r.ToDto())
                .ToListAsync();
        }
    }
}