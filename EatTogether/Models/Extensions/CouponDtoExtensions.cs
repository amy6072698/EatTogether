using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;

namespace EatTogether.Models.Extensions
{
    public static class CouponDtoExtensions
    {
        // EFModel → DTO
        public static CouponDto ToDto(this Coupon c)
        {
            return new CouponDto
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                DiscountType = c.DiscountType,
                DiscountValue = c.DiscountValue,
                MinSpend = c.MinSpend,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                LimitCount = c.LimitCount,
                ReceivedCount = c.ReceivedCount ?? 0
            };
        }
    }
}
