using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;

namespace EatTogether.Models.Extensions
{
    public static class ReservationDtoExtensions
    {
        // EFModel → DTO
        public static ReservationDto ToDto(this Reservation r)
        {
            return new ReservationDto
            {
                Id = r.Id,
                BookingNumber = r.BookingNumber,
                Name = r.Name,
                Phone = r.Phone,
                Email = r.Email,
                ReservationDate = r.ReservationDate,
                AdultsCount = r.AdultsCount,
                ChildrenCount = r.ChildrenCount,
                Status = r.Status,
                Remark = r.Remark,
                ReservedAt = r.ReservedAt
            };
        }
    }
}
