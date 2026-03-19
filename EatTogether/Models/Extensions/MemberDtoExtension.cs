using EatTogether.Models.DTOs;
using EatTogether.Models.ViewModels;

namespace EatTogether.Models.Extensions
{
	public static class MemberDtoExtension
	{
		// 狀態判斷邏輯（集中於此），優先順序：IsDeleted → IsBlacklisted → !IsConfirmed → 啟用中
		private static (string text, string color) ResolveStatus(MemberListDto dto)
		{
			if (dto.IsDeleted) return ("已刪除", "deleted");
			if (dto.IsBlacklisted) return ("黑名單", "blacklisted");
			if (!dto.IsConfirmed) return ("未驗證", "unverified");
			return ("啟用中", "active");
		}


		// 操作按鈕類型
		//   blacklist   → 啟用中 / 未驗證（加入黑名單，紅色可點）
		//   unblacklist → 黑名單（解除黑名單，綠色可點）
		//   disabled    → 已刪除（加入黑名單灰色 Disabled，解除不顯示）
		private static string ResolveButtonType(MemberListDto dto)
		{
			if (dto.IsDeleted) return "disabled";
			if (dto.IsBlacklisted) return "unblacklist";
			return "blacklist";
		}


		// MemberListDto → MemberRowViewModel
		public static MemberRowViewModel ToRowVm(this MemberListDto dto)
		{
			var (statusText, statusColor) = ResolveStatus(dto);

			return new MemberRowViewModel
			{
				Id = dto.Id,
				Name = dto.Name,
				Account = dto.Account,
				Email = dto.Email,
				Phone = dto.Phone,
				CreatedAt = dto.CreatedAt,
				DeletedAt = dto.DeletedAt,
				BirthDate = dto.BirthDate,
				BlacklistReason = dto.BlacklistReason,
				StatusText = statusText,
				StatusColor = statusColor,
				ButtonType = ResolveButtonType(dto),
			};
		}

		// MemberDetailDto → MemberDetailViewModel
		public static MemberDetailViewModel ToDetailVm(this MemberDetailDto dto)
		{
			var (statusText, _) = ResolveStatus(dto);

			// 黑名單原因：僅黑名單狀態顯示；NULL 顯示「（未填寫）」
			string? blacklistReason = null;
			if (dto.IsBlacklisted && !dto.IsDeleted)
			{
				blacklistReason = string.IsNullOrWhiteSpace(dto.BlacklistReason)
					? "（未填寫）"
					: dto.BlacklistReason;
			}

			return new MemberDetailViewModel
			{
				Id = dto.Id,
				Name = dto.Name,
				Account = dto.Account,
				Email = dto.Email,
				Phone = dto.Phone,
				BirthDate = dto.BirthDate?.ToString("yyyy-MM-dd"),
				CreatedAt = dto.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
				DeletedAt = dto.DeletedAt?.ToString("yyyy-MM-dd HH:mm:ss"),
				StatusText = statusText,
				BlacklistReason = blacklistReason,
			};
		}
	}
}
