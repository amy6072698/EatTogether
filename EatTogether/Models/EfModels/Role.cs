using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class Role
{
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<RoleFunction> RoleFunctions { get; set; } = new List<RoleFunction>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
