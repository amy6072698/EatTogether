using EatTogether.Models.DTOs;

namespace EatTogether.Models.ViewModels
{
    public static class TableViewModelExtension
    {
        public static TableDto ToDto(this TableCreateViewModel vm)
        {
            return new TableDto
            {
                TableName = vm.TableName,
                SeatCount = vm.SeatCount
            };
        }
    }

    public static class ReservationViewModelExtension
    {
        public static ReservationDto ToDto(this ReservationCreateViewModel vm)
        {
            return new ReservationDto
            {
                Name = vm.Name,
                Phone = vm.Phone,
                Email = vm.Email,
                ReservationDate = vm.ReservationDate,
                AdultsCount = vm.AdultsCount,
                ChildrenCount = vm.ChildrenCount,
                Remark = vm.Remark
            };
        }
    }

    public static class CouponViewModelExtension
    {
        public static CouponDto ToDto(this CouponCreateViewModel vm)
        {
            return new CouponDto
            {
                Name = vm.Name,
                Code = vm.Code.ToUpper(),
                DiscountType = vm.DiscountType,
                DiscountValue = vm.DiscountValue,
                MinSpend = vm.MinSpend,
                StartDate = vm.StartDate,
                EndDate = vm.EndDate,
                LimitCount = vm.LimitCount
            };
        }
    }
}
