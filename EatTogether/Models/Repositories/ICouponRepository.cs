using EatTogether.Models.DTOs;

namespace EatTogether.Models.Repositories
{
    public interface ICouponRepository
    {
        Task<IEnumerable<CouponDto>> GetAllAsync();
        Task<List<CouponDto>> GetApplicableCouponsAsync(int amount);
        /// <summary>依 ID 清單取回優惠券 DTO（不限門檻），用於顯示使用中但已不符目前金額門檻的優惠券。</summary>
        Task<List<CouponDto>> GetCouponsByIdsAsync(IEnumerable<int> ids);
        Task<CouponDto?> GetByIdAsync(int id);
        Task<CouponDto?> GetByCodeAsync(string code);
        Task CreateAsync(CouponDto dto);
        Task<bool> IsCodeExistsAsync(string code);
        Task IncrementReceivedCountAsync(int id);
        Task UpdateNameAsync(int id, string newName);
        Task AddLimitCountAsync(int id, int amount);
        Task DisableAsync(int id);
        Task EnableAsync(int id);
        Task UpdateEndDateAsync(int id, DateTime? newEndDate);
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
