using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class User
{
    public int Id { get; set; }

    public string Account { get; set; } = null!;

    public byte[] Password { get; set; } = null!;

    public string EmployeeNumber { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public DateOnly HireDate { get; set; }

    public DateOnly? TerminationDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<PreOrder> PreOrders { get; set; } = new List<PreOrder>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
