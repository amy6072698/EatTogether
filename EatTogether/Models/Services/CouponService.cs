using EatTogether.Models.DTOs;
using EatTogether.Models.Infra;
using EatTogether.Models.Repositories;

namespace EatTogether.Models.Services
{
    public class CouponService
    {
        private readonly ICouponRepository _couponRepo;
        private readonly IMemberCouponRepository _memberCouponRepo;

        public CouponService(ICouponRepository couponRepo, IMemberCouponRepository memberCouponRepo)
        {
            _couponRepo = couponRepo;
            _memberCouponRepo = memberCouponRepo;
        }

        public async Task<IEnumerable<CouponDto>> GetAllAsync() => await _couponRepo.GetAllAsync();

        public async Task<CouponDto?> GetByIdAsync(int id) => await _couponRepo.GetByIdAsync(id);

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

        public async Task<Result> CreateAsync(CouponDto dto)
        {
            if (await _couponRepo.IsCodeExistsAsync(dto.Code))
                return Result.Fail($"折扣碼「{dto.Code}」已存在，請更換其他折扣碼");
            await _couponRepo.CreateAsync(dto);
            return Result.Success();
        }

        public async Task<IEnumerable<MemberCouponDto>> GetAllMemberCouponsAsync()
            => await _memberCouponRepo.GetAllAsync();

        public async Task<(Result result, int discountAmount)> RedeemCouponAsync(string code, int memberId, int orderAmount)
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
