namespace EatTogether.Models.DTOs
{
    public class CategoryDto
    {
        // Id CategoryName IsActive ParentCategoryId  ParentCategoryName DisplayOrder ImageUrl CreatedAt UpdateAt
        public int Id { get; set; }

        public string CategoryName { get; set; } = null!;
        public bool IsActive { get; set; }

        public int? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }

        public int DisplayOrder { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
