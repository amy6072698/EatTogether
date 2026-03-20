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

        public async Task<List<CouponDto>> GetApplicableCouponsAsync(int amount)
        {
            var today = DateTime.Today;
            return await _context.Coupons
                .AsNoTracking()
                .Where(c => !c.IsDisabled
                         && c.StartDate <= today
                         && (c.EndDate == null || c.EndDate >= today)
                         && c.MinSpend <= amount)
                .OrderByDescending(c => c.MinSpend)
                .Select(c => c.ToDto())
                .ToListAsync();
        }

        public async Task<List<CouponDto>> GetCouponsByIdsAsync(IEnumerable<int> ids)
        {
            var idSet = ids.ToHashSet();
            if (idSet.Count == 0) return new List<CouponDto>();

            return await _context.Coupons
                .AsNoTracking()
                .Where(c => idSet.Contains(c.Id))
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

        public async Task UpdateNameAsync(int id, string newName)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return;
            coupon.Name = newName;
            await _context.SaveChangesAsync();
        }

        public async Task AddLimitCountAsync(int id, int amount)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return;
            coupon.LimitCount = (coupon.LimitCount ?? 0) + amount;
            await _context.SaveChangesAsync();
        }

        public async Task DisableAsync(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return;
            coupon.IsDisabled = true;
            await _context.SaveChangesAsync();
        }

        public async Task EnableAsync(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return;
            coupon.IsDisabled = false;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEndDateAsync(int id, DateTime? newEndDate)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return;
            coupon.EndDate = newEndDate;
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
                    UsedDate = mc.UsedDate,
                    ClaimedAt = mc.ClaimedAt
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
                UsedDate = mc.UsedDate,
                ClaimedAt = mc.ClaimedAt
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
                    UsedDate = mc.UsedDate,
                    ClaimedAt = mc.ClaimedAt
                })
                .ToListAsync();
        }

        public async Task AddAsync(int memberId, int couponId)
        {
            var mc = new MemberCoupon
            {
                MemberId = memberId,
                CouponId = couponId,
                IsUsed = false,
                ClaimedAt = DateTime.Now
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
