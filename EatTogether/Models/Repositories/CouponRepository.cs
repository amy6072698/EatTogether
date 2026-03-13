using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly EatTogetherDBContext _context;

        public CouponRepository(EatTogetherDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CouponDto>> GetAllAsync()
        {
            return await _context.Coupons
                .OrderByDescending(c => c.StartDate)
                .Select(c => c.ToDto())
                .ToListAsync();
        }

        public async Task<CouponDto?> GetByIdAsync(int id)
        {
            var c = await _context.Coupons.FindAsync(id);
            return c?.ToDto();
        }

        public async Task<CouponDto?> GetByCodeAsync(string code)
        {
            var c = await _context.Coupons
                .FirstOrDefaultAsync(x => x.Code == code);
            return c?.ToDto();
        }

        public async Task CreateAsync(CouponDto dto)
        {
            var coupon = new Coupon
            {
                Name = dto.Name,
                Code = dto.Code.ToUpper(),
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                MinSpend = dto.MinSpend,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                LimitCount = dto.LimitCount,
                ReceivedCount = 0
            };
            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsCodeExistsAsync(string code)
        {
            return await _context.Coupons
                .AnyAsync(c => c.Code == code.ToUpper());
        }

        public async Task IncrementReceivedCountAsync(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return;

            coupon.ReceivedCount = (coupon.ReceivedCount ?? 0) + 1;
            await _context.SaveChangesAsync();
        }
    }

    public class MemberCouponRepository : IMemberCouponRepository
    {
        private readonly EatTogetherDBContext _context;

        public MemberCouponRepository(EatTogetherDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MemberCouponDto>> GetAllAsync()
        {
            return await _context.MemberCoupons
                .Include(mc => mc.Member)
                .Include(mc => mc.Coupon)
                .OrderByDescending(mc => mc.Id)
                .Select(mc => new MemberCouponDto
                {
                    Id = mc.Id,
                    MemberId = mc.MemberId,
                    MemberName = mc.Member.Name,
                    MemberAccount = mc.Member.Account,
                    CouponId = mc.CouponId,
                    CouponName = mc.Coupon.Name,
                    Code = mc.Coupon.Code,
                    DiscountType = mc.Coupon.DiscountType,
                    DiscountValue = mc.Coupon.DiscountValue,
                    EndDate = mc.Coupon.EndDate,
                    IsUsed = mc.IsUsed,
                    UsedDate = mc.UsedDate
                })
                .ToListAsync();
        }

        public async Task<MemberCouponDto?> GetByMemberAndCouponAsync(int memberId, int couponId)
        {
            var mc = await _context.MemberCoupons
                .Include(x => x.Coupon)
                .Include(x => x.Member)
                .FirstOrDefaultAsync(x => x.MemberId == memberId && x.CouponId == couponId);

            if (mc == null) return null;

            return new MemberCouponDto
            {
                Id = mc.Id,
                MemberId = mc.MemberId,
                MemberName = mc.Member.Name,
                MemberAccount = mc.Member.Account,
                CouponId = mc.CouponId,
                CouponName = mc.Coupon.Name,
                Code = mc.Coupon.Code,
                DiscountType = mc.Coupon.DiscountType,
                DiscountValue = mc.Coupon.DiscountValue,
                EndDate = mc.Coupon.EndDate,
                IsUsed = mc.IsUsed,
                UsedDate = mc.UsedDate
            };
        }

        public async Task<IEnumerable<MemberCouponDto>> GetByMemberAsync(int memberId)
        {
            return await _context.MemberCoupons
                .Include(mc => mc.Coupon)
                .Include(mc => mc.Member)
                .Where(mc => mc.MemberId == memberId)
                .Select(mc => new MemberCouponDto
                {
                    Id = mc.Id,
                    MemberId = mc.MemberId,
                    MemberName = mc.Member.Name,
                    MemberAccount = mc.Member.Account,
                    CouponId = mc.CouponId,
                    CouponName = mc.Coupon.Name,
                    Code = mc.Coupon.Code,
                    DiscountType = mc.Coupon.DiscountType,
                    DiscountValue = mc.Coupon.DiscountValue,
                    EndDate = mc.Coupon.EndDate,
                    IsUsed = mc.IsUsed,
                    UsedDate = mc.UsedDate
                })
                .ToListAsync();
        }

        public async Task AddAsync(int memberId, int couponId)
        {
            var mc = new MemberCoupon
            {
                MemberId = memberId,
                CouponId = couponId,
                IsUsed = false
            };
            _context.MemberCoupons.Add(mc);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsUsedAsync(int memberId, int couponId)
        {
            var mc = await _context.MemberCoupons
                .FirstOrDefaultAsync(x => x.MemberId == memberId && x.CouponId == couponId);
            if (mc == null) return;

            mc.IsUsed = true;
            mc.UsedDate = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }
}
