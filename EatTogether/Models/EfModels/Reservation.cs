using System;
using System.Collections.Generic;

namespace EatTogether.Models.EfModels;

public partial class Reservation
{
    public int Id { get; set; }

    public string BookingNumber { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime ReservationDate { get; set; }

    public int AdultsCount { get; set; }

    public int ChildrenCount { get; set; }

    public int Status { get; set; }

    public string? Remark { get; set; }

    public DateTime ReservedAt { get; set; }
}
