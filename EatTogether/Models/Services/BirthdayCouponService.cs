using EatTogether.Models.EfModels;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Services
{
    public class BirthdayCouponService
    {
        private readonly EatTogetherDBContext _context;
        private readonly ReservationEmailService _emailService;

        private const int BIRTHDAY_DISCOUNT_VALUE = 150; // 折 $150
        private const int BIRTHDAY_MIN_SPEND = 500; // 最低消費 $500

        public BirthdayCouponService(EatTogetherDBContext context, ReservationEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        /// <summary>
        /// 每月1號統一發放當月生日優惠券
        /// 規則：
        ///   - 每月只建立一張共用優惠券（折扣碼格式：BDAYyymm）
        ///   - 找出當月所有壽星，各自新增一筆 MemberCoupon 紀錄
        ///   - 已領過的會員跳過，不重複發放
        ///   - 限當月底前使用
        /// </summary>
        public async Task<int> IssueBirthdayCouponsAsync()
        {
            var today = DateTime.Today;
            var monthEnd = new DateTime(today.Year, today.Month,
                DateTime.DaysInMonth(today.Year, today.Month), 23, 59, 59);
            var monthStart = new DateTime(today.Year, today.Month, 1);

            // 當月共用折扣碼：BDAY + 年後2碼 + 月2碼（例：BDAY2603）
            var sharedCode = $"BDAY{today.Year % 100:D2}{today.Month:D2}";
            var couponName = $"{today.Year}年{today.Month}月生日優惠";

            // ① 取得或建立當月共用優惠券
            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == sharedCode);

            if (coupon == null)
            {
                coupon = new Coupon
                {
                    Name = couponName,
                    Code = sharedCode,
                    DiscountType = 0,
                    DiscountValue = BIRTHDAY_DISCOUNT_VALUE,
                    MinSpend = BIRTHDAY_MIN_SPEND,
                    StartDate = monthStart,
                    EndDate = monthEnd,
                    LimitCount = null,   // 無限量（每位壽星各用一次）
                    ReceivedCount = 0
                };
                _context.Coupons.Add(coupon);
                await _context.SaveChangesAsync();
            }

            // ② 找出當月生日的有效會員
            var birthdayMembers = await _context.Members
                .Where(m => !m.IsDeleted && !m.IsBlacklisted
                         && m.BirthDate.HasValue
                         && m.BirthDate.Value.Month == today.Month)
                .ToListAsync();

            // ③ 找出本月已領過的 MemberId
            var alreadyIssuedIds = await _context.MemberCoupons
                .Where(mc => mc.CouponId == coupon.Id)
                .Select(mc => mc.MemberId)
                .ToListAsync();

            int issued = 0;
            foreach (var member in birthdayMembers)
            {
                if (alreadyIssuedIds.Contains(member.Id)) continue;

                // 新增 MemberCoupon 紀錄
                _context.MemberCoupons.Add(new MemberCoupon
                {
                    MemberId = member.Id,
                    CouponId = coupon.Id,
                    IsUsed = false,
                    ClaimedAt = DateTime.Now
                });
                coupon.ReceivedCount = (coupon.ReceivedCount ?? 0) + 1;
                await _context.SaveChangesAsync();

                // 寄發通知信
                if (!string.IsNullOrEmpty(member.Email))
                {
                    var body = _emailService.BuildBirthdayCouponEmail(
                        member.Name, sharedCode, BIRTHDAY_DISCOUNT_VALUE, monthEnd);
                    await _emailService.EnqueueAsync(
                        member.Email,
                        $"🎂 {member.Name}，您的義起吃 {today.Month}月生日專屬優惠券來了！",
                        body);
                }
                issued++;
            }
            return issued;
        }
    }
}
