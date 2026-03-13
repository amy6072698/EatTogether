using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.ViewModels
{
    public class SetMealViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "套餐名稱為必填")]
        [StringLength(100, ErrorMessage = "套餐名稱最多 100 字")]
        [Display(Name = "套餐名稱")]
        public string SetMealName { get; set; } = null!;

        [Required(ErrorMessage = "請選擇折扣類型")]
        [Display(Name = "折扣類型")]
        public string DiscountType { get; set; } = "percent";

        [Required(ErrorMessage = "折扣值為必填")]
        [Range(0, 99999, ErrorMessage = "折扣值請填 0~99999")]
        [Display(Name = "折扣值")]
        public decimal DiscountValue { get; set; }

        [Display(Name = "是否啟用")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "套餐定價")]
        [Range(0, 99999, ErrorMessage = "定價請填 0~99999")]
        public decimal? SetPrice { get; set; }

        [StringLength(300, ErrorMessage = "描述最多 300 字")]
        [Display(Name = "描述")]
        public string? Description { get; set; }

        [Display(Name = "圖片網址")]
        public string? ImageUrl { get; set; }

        [Display(Name = "折扣開始日期")]
        [DataType(DataType.Date)]
        public DateOnly? StartDate { get; set; }

        [Display(Name = "折扣結束日期")]
        [DataType(DataType.Date)]
        public DateOnly? EndDate { get; set; }

        // 裁切後的 Base64 圖片資料
        public string? CroppedImageData { get; set; }

        [Display(Name = "建立時間")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "更新時間")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "顯示順序")]
        public int DisplayOrder { get; set; }

        public List<SetMealItemViewModel> Items { get; set; } = new();

        public List<SelectListItem> DiscountTypeOptions { get; set; } = new()
        {
            new SelectListItem { Value = "percent", Text = "百分比折扣（%）" },
            new SelectListItem { Value = "fixed",   Text = "固定折扣（元）" }
        };
    }

    public class SetMealItemViewModel
    {
        public int Id { get; set; }
        public int SetMealId { get; set; }

        [Required(ErrorMessage = "請選擇餐點")]
        [Display(Name = "餐點")]
        public int DishId { get; set; }

        [Display(Name = "餐點名稱")]
        public string? DishName { get; set; }

        [Display(Name = "餐點單價")]
        public decimal? DishPrice { get; set; }

        [Required(ErrorMessage = "數量為必填")]
        [Range(1, 99, ErrorMessage = "數量請填 1~99")]
        [Display(Name = "數量")]
        public int Quantity { get; set; } = 1;

        [Display(Name = "是否為選擇性項目")]
        public bool IsOptional { get; set; }

        [Display(Name = "選項群組編號")]
        public int? OptionGroupNo { get; set; }

        [Display(Name = "選取上限")]
        public int? PickLimit { get; set; }

        [Display(Name = "顯示順序")]
        public int DisplayOrder { get; set; } = 1;

        public List<SelectListItem> DishOptions { get; set; } = new();
    }
}
