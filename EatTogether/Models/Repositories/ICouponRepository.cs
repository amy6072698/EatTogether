using EatTogether.Models.DTOs;

namespace EatTogether.Models.Repositories
{
    public interface ICouponRepository
    {
        Task<IEnumerable<CouponDto>> GetAllAsync();
        Task<CouponDto?> GetByIdAsync(int id);
        Task<CouponDto?> GetByCodeAsync(string code);
        Task CreateAsync(CouponDto dto);
        Task<bool> IsCodeExistsAsync(string code);
        Task IncrementReceivedCountAsync(int id);
        Task UpdateNameAsync(int id, string newName);
        Task AddLimitCountAsync(int id, int amount);
    }

    public interface IMemberCouponRepository
    {
        Task<IEnumerable<MemberCouponDto>> GetAllAsync();
        Task<MemberCouponDto?> GetByMemberAndCouponAsync(int memberId, int couponId);
        Task<IEnumerable<MemberCouponDto>> GetByMemberAsync(int memberId);
        Task AddAsync(int memberId, int couponId);
        Task MarkAsUsedAsync(int memberId, int couponId);
    }
}
