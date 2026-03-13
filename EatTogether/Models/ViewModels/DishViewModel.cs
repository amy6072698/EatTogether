using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.ViewModels
{
    public class DishViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "請選擇分類")]
        [Display(Name = "分類")]
        public int CategoryId { get; set; }

        [Display(Name = "分類名稱")]
        public string? CategoryName { get; set; }

        [Required(ErrorMessage = "餐點名稱為必填")]
        [StringLength(100, ErrorMessage = "餐點名稱最多 100 字")]
        [Display(Name = "餐點名稱")]
        public string DishName { get; set; } = null!;

        [Required(ErrorMessage = "價格為必填")]
        [Range(0, 99999, ErrorMessage = "價格請填 0~99999")]
        [Display(Name = "價格")]
        public decimal Price { get; set; }

        [Display(Name = "是否啟用")]
        public bool IsActive { get; set; } = true;

        [StringLength(300, ErrorMessage = "描述最多 300 字")]
        [Display(Name = "描述")]
        public string? Description { get; set; }

        [Display(Name = "圖片網址")]
        public string? ImageUrl { get; set; }

        // 裁切後的 Base64 圖片資料
        public string? CroppedImageData { get; set; }

        [Display(Name = "可外帶")]
        public bool IsTakeOut { get; set; }

        [Display(Name = "限定供應")]
        public bool IsLimited { get; set; }

        [Display(Name = "供應開始日")]
        public DateOnly? StartDate { get; set; }

        [Display(Name = "供應結束日")]
        public DateOnly? EndDate { get; set; }

        [Display(Name = "建立時間")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "更新時間")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "顯示順序")]
        public int DisplayOrder { get; set; }

        // 給下拉選單用
        public List<SelectListItem> CategoryOptions { get; set; } = new();
    }
}
