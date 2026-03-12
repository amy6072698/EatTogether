using System.ComponentModel.DataAnnotations;

namespace EatTogether.Models.ViewModels
{
    public class ReservationCreateViewModel
    {
        [Required(ErrorMessage = "姓名為必填")]
        [StringLength(50, ErrorMessage = "姓名最多 50 個字元")]
        [Display(Name = "姓名")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "電話為必填")]
        [StringLength(20, ErrorMessage = "電話最多 20 個字元")]
        [Display(Name = "電話")]
        public string Phone { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Email 格式不正確")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "預約日期時間為必填")]
        [Display(Name = "預約日期時間")]
        public DateTime ReservationDate { get; set; }

        [Required(ErrorMessage = "大人人數為必填")]
        [Range(1, 99, ErrorMessage = "大人人數至少 1 人")]
        [Display(Name = "大人人數")]
        public int AdultsCount { get; set; } = 2;

        [Range(0, 99, ErrorMessage = "小孩人數不可為負數")]
        [Display(Name = "小孩人數")]
        public int ChildrenCount { get; set; } = 0;

        [StringLength(200)]
        [Display(Name = "備註")]
        public string? Remark { get; set; }
    }

    public class ReservationSearchViewModel
    {
        public DateTime? SearchDate { get; set; }
        public int? StatusFilter { get; set; }
        public IEnumerable<DTOs.ReservationDto> Results { get; set; } = new List<DTOs.ReservationDto>();
    }

    public class ReservationUpdateStatusViewModel
    {
        public int Id { get; set; }
        public int Status { get; set; }
    }
}
