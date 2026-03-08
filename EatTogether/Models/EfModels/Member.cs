using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class Member
{
    public int Id { get; set; }

    public string Account { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public byte[] Password { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public bool IsBlacklisted { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<MemberCoupon> MemberCoupons { get; set; } = new List<MemberCoupon>();

    public virtual ICollection<MemberFavorite> MemberFavorites { get; set; } = new List<MemberFavorite>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<PreOrder> PreOrders { get; set; } = new List<PreOrder>();

    public virtual ICollection<SubscriptionPreference> SubscriptionPreferences { get; set; } = new List<SubscriptionPreference>();

    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}
