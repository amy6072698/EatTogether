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
            {
                // 先在 Scope 內查好資料，再丟背景執行（避免 DbContext 被 dispose）
                var members = await _context.Members
                    .Where(m => !m.IsDeleted && !m.IsBlacklisted
                             && !string.IsNullOrEmpty(m.Email))
                    .Select(m => new { m.Name, m.Email })
                    .ToListAsync();

                var discountDesc = dto.DiscountType == 0
                    ? $"折 ${dto.DiscountValue}"
                    : $"打 {100 - dto.DiscountValue} 折";

                // 背景發送：所有資料已取出，不依賴 DbContext
                _ = Task.Run(async () =>
                {
                    foreach (var member in members)
                    {
                        try
                        {
                            var body = _emailService.BuildCouponNotifyEmail(
                                member.Name, dto.Name, dto.Code,
                                discountDesc, dto.MinSpend, dto.EndDate);

                            await _emailService.SendOnlyAsync(
                                member.Email,
                                $"🎉 義起吃新優惠上線！{dto.Name}，快來領取",
                                body);
                        }
                        catch { /* 單筆失敗不影響其他 */ }
                    }
                });
            }

            return Result.Success();
        }


        /// <summary>
        /// 一鍵發放優惠券給全會員
        /// 條件：無限量（LimitCount=NULL）、非生日類型、有效中、未停用
        /// 已領過的會員跳過
        /// </summary>
        public async Task<(int issued, int skipped)> IssueToAllMembersAsync(int couponId)
        {
            var coupon = await _couponRepo.GetByIdAsync(couponId);
            if (coupon == null) return (0, 0);

            // 找出所有有效會員
            var allMembers = await _context.Members
                .Where(m => !m.IsDeleted && !m.IsBlacklisted)
                .Select(m => m.Id)
                .ToListAsync();

            // 已領過此券的 MemberId
            var alreadyClaimed = await _context.MemberCoupons
                .Where(mc => mc.CouponId == couponId)
                .Select(mc => mc.MemberId)
                .ToListAsync();

            int issued = 0, skipped = 0;
            foreach (var memberId in allMembers)
            {
                if (alreadyClaimed.Contains(memberId))
                {
                    skipped++;
                    continue;
                }
                _context.MemberCoupons.Add(new EatTogether.Models.EfModels.MemberCoupon
                {
                    MemberId = memberId,
                    CouponId = couponId,
                    IsUsed = false,
                    ClaimedAt = DateTime.Now
                });
                issued++;
            }
            if (issued > 0)
            {
                await _context.SaveChangesAsync();
                // 直接更新 ReceivedCount += issued（不用補正）
                var couponEntity = await _context.Coupons.FindAsync(couponId);
                if (couponEntity != null)
                {
                    couponEntity.ReceivedCount = (couponEntity.ReceivedCount ?? 0) + issued;
                    await _context.SaveChangesAsync();
                }
            }
            return (issued, skipped);
        }

        public async Task<Result> DisableAsync(int id)
        {
            var coupon = await _couponRepo.GetByIdAsync(id);
            if (coupon == null) return Result.Fail("找不到此優惠券");
            if (coupon.IsDisabled) return Result.Fail("此優惠券已是停用狀態");
            await _couponRepo.DisableAsync(id);
            return Result.Success();
        }

        public async Task<Result> EnableAsync(int id)
        {
            var coupon = await _couponRepo.GetByIdAsync(id);
            if (coupon == null) return Result.Fail("找不到此優惠券");
            if (!coupon.IsDisabled) return Result.Fail("此優惠券已是啟用狀態");
            await _couponRepo.EnableAsync(id);
            return Result.Success();
        }



        public async Task<Result> EditAsync(int id, string newName, int? addLimitCount, DateTime? newEndDate)
        {
            var coupon = await _couponRepo.GetByIdAsync(id);
            if (coupon == null) return Result.Fail("找不到此優惠券");

            if (!string.IsNullOrWhiteSpace(newName))
                await _couponRepo.UpdateNameAsync(id, newName.Trim());

            if (addLimitCount.HasValue && addLimitCount.Value > 0)
                await _couponRepo.AddLimitCountAsync(id, addLimitCount.Value);

            // 有效期間：有填新結束日才更新（設為當天 23:59:59）
            if (newEndDate.HasValue)
            {
                if (newEndDate.Value.Date < coupon.StartDate.Date)
                    return Result.Fail("結束日期不能早於開始日期");
                await _couponRepo.UpdateEndDateAsync(id,
                    newEndDate.Value.Date.AddDays(1).AddSeconds(-1));
            }

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

            if (coupon.IsDisabled)
                return (Result.Fail("此優惠券已停用"), 0);

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
