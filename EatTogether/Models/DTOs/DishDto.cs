namespace EatTogether.Models.DTOs
{
    public class DishDto
    {
        // Id CategoryId CategoryName DishName Price IsActive Description ImageUrl IsTakeOut IsLimited StartDate EndDate CreatedAt UpdatedAt
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string DishName { get; set; } = null!;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsTakeOut { get; set; }
        public bool IsLimited { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int DisplayOrder { get; set; }
    }
}
