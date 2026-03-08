using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class Function
{
    public int Id { get; set; }

    public string Category { get; set; } = null!;

    public string FunctionName { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<RoleFunction> RoleFunctions { get; set; } = new List<RoleFunction>();
}
