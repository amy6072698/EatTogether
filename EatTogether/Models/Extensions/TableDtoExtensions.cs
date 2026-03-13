using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;

namespace EatTogether.Models.Extensions
{
    public static class TableDtoExtensions
    {
        // EFModel → DTO
        public static TableDto ToDto(this Table table)
        {
            return new TableDto
            {
                Id = table.Id,
                TableName = table.TableName,
                SeatCount = table.SeatCount,
                Status = table.Status
            };
        }
    }
}
