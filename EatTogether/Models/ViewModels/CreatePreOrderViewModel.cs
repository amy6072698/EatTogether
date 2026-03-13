using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.ViewModels
{
    public class CreatePreOrderViewModel
    {
        public int TableId { get; set; }
        public bool InOrOut { get; set; } = true;   // 預設內用
        public List<SelectListItem> TableOptions { get; set; } = new();
        public string? Note { get; set; }
        public int DiscountAmount { get; set; }
        public List<CreatePreOrderItemViewModel> Items { get; set; } = new();
    }
}
