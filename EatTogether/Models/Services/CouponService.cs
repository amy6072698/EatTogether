using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.Infra;
using EatTogether.Models.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Services
{
    public class CouponService
    {
        private readonly ICouponRepository _couponRepo;
        private readonly IMemberCouponRepository _memberCouponRepo;
        private readonly EatTogetherDBContext _context;
        private readonly ReservationEmailService _emailService;

        public CouponService(
            ICouponRepository couponRepo,
            IMemberCouponRepository memberCouponRepo,
            EatTogetherDBContext context,
            ReservationEmailService emailService)
        {
            _couponRepo = couponRepo;
            _memberCouponRepo = memberCouponRepo;
            _context = context;
            _emailService = emailService;
        }

        public async Task<IEnumerable<CouponDto>> GetAllAsync() => await _couponRepo.GetAllAsync();

        public async Task<CouponDto?> GetByIdAsync(int id) => await _couponRepo.GetByIdAsync(id);

        public async Task<Result> CreateAsync(CouponDto dto)
        {
            if (await _couponRepo.IsCodeExistsAsync(dto.Code))
                return Result.Fail($"折扣碼「{dto.Code}」已存在，請更換其他折扣碼");

            await _couponRepo.CreateAsync(dto);

            // ── 當場啟用（StartDate <= 現在 且未過期）→ 立即通知所有會員 ──
            var now = DateTime.Now;
            bool isActiveNow = dto.StartDate <= now
                            && (!dto.EndDate.HasValue || dto.EndDate.Value >= now);

            if (isActiveNow)
                await NotifyAllMembersAsync(dto);

            return Result.Success();
        }

        /// <summary>新優惠券立即通知所有有效會員</summary>
        private async Task NotifyAllMembersAsync(CouponDto dto)
        {
            var members = await _context.Members
                .Where(m => !m.IsDeleted && !m.IsBlacklisted
                         && !string.IsNullOrEmpty(m.Email))
                .ToListAsync();

            var discountDesc = dto.DiscountType == 0
                ? $"折 ${dto.DiscountValue}"
                : $"打 {100 - dto.DiscountValue} 折";

            foreach (var member in members)
            {
                var body = _emailService.BuildCouponNotifyEmail(
                    member.Name,
                    dto.Name,
                    dto.Code,
                    discountDesc,
                    dto.MinSpend,
                    dto.EndDate);

                await _emailService.EnqueueAsync(
                    member.Email,
                    $"🎉 義起吃新優惠上線！{dto.Name}，快來領取",
                    body);
            }
        }

        public async Task<Result> EditAsync(int id, string newName, int? addLimitCount)
        {
            var coupon = await _couponRepo.GetByIdAsync(id);
            if (coupon == null) return Result.Fail("找不到此優惠券");

            if (!string.IsNullOrWhiteSpace(newName))
                await _couponRepo.UpdateNameAsync(id, newName.Trim());

            if (addLimitCount.HasValue && addLimitCount.Value > 0)
                await _couponRepo.AddLimitCountAsync(id, addLimitCount.Value);

            return Result.Success();
        }

        public async Task<IEnumerable<MemberCouponDto>> GetAllMemberCouponsAsync()
            => await _memberCouponRepo.GetAllAsync();

        public async Task<(Result result, int discountAmount)> RedeemCouponAsync(
            string code, int memberId, int orderAmount)
        {
            var coupon = await _couponRepo.GetByCodeAsync(code);
            if (coupon == null)
                return (Result.Fail("折扣碼不存在"), 0);

            if (DateTime.Now < coupon.StartDate)
                return (Result.Fail("此優惠活動尚未開始"), 0);

            if (coupon.EndDate.HasValue && DateTime.Now > coupon.EndDate.Value)
                return (Result.Fail("此優惠券已過期"), 0);

            if (coupon.LimitCount.HasValue && coupon.ReceivedCount >= coupon.LimitCount.Value)
                return (Result.Fail("此優惠券已達領取上限"), 0);

            if (orderAmount < coupon.MinSpend)
                return (Result.Fail($"未達最低消費門檻 ${coupon.MinSpend}"), 0);

            var record = await _memberCouponRepo.GetByMemberAndCouponAsync(memberId, coupon.Id);
            if (record != null && record.IsUsed)
                return (Result.Fail("此優惠券已使用過"), 0);

            int discount = coupon.DiscountType == 0
                ? coupon.DiscountValue
                : (int)(orderAmount * coupon.DiscountValue / 100.0);

            if (record == null)
                await _memberCouponRepo.AddAsync(memberId, coupon.Id);
            await _memberCouponRepo.MarkAsUsedAsync(memberId, coupon.Id);
            await _couponRepo.IncrementReceivedCountAsync(coupon.Id);

            return (Result.Success(), discount);
        }
    }
}
