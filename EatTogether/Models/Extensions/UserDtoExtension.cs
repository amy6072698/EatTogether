using EatTogether.Models.DTOs;
using EatTogether.Models.ViewModels;

namespace EatTogether.Models.Extensions
{
	public static class UserDtoExtension
	{
		public static UserRowViewModel ToRowVm(this UserListDto dto)
		{
			return new UserRowViewModel
			{
				Id = dto.Id,
				EmployeeNumber = dto.EmployeeNumber,
				Name = dto.Name,
				Account = dto.Account,
				Email = dto.Email,
				Phone = dto.Phone,
				HireDate = dto.HireDate,
				CreatedAt = dto.CreatedAt,
				RoleIds = dto.RoleIds,
				RoleNames = dto.RoleNames,
				IsDeleted = dto.IsDeleted,
				IsActive = dto.IsActive,
				CanEdit = dto.CanEdit,
				CanResign = dto.CanResign,
				CanReinstate = dto.CanReinstate
			};
		}
		public static UserEditViewModel ToEditVm(this UserEditDto dto)
		{
			return new UserEditViewModel
			{
				Id = dto.Id,
				EmployeeNumber = dto.EmployeeNumber,
				CreatedAt = dto.CreatedAt,
				Name = dto.Name,
				Account = dto.Account,
				Email = dto.Email,
				Phone = dto.Phone,
				HireDate = dto.HireDate,
				IsActive = dto.IsActive,
				RoleIds = dto.RoleIds
				// Password 不預填
			};
		}
	}
}
