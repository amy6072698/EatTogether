using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.ViewModels
{
    public class CreatePreOrderItemViewModel
    {
        public int ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }

        public int Qty { get; set; }

        public int UnitPrice { get; set; }
    }
}
