using EatTogether.Models.EfModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.Common;

namespace EatTogether.Models.Infra
{
	/// <summary>
	/// 員工編號產生器。
	/// 只負責編號產生邏輯，不存取資料庫。
	/// 格式：EMP + 年份(4碼) + 流水號(3碼)，如 EMP2025001
	/// </summary>
	public class UserNumberGenerator
	{
		/// <summary>
		/// 根據當年度最後一個員工編號，產生下一個編號
		/// </summary>
		/// <param name="lastEmployeeNumber">Repository 查到的最後一個編號，無資料傳空字串</param>
		public string Generate(string lastEmployeeNumber)
		{
			int currentYear = DateTime.Now.Year;
			string prefix = $"EMP{currentYear}";

			int nextNumber = 1;

			if (!string.IsNullOrEmpty(lastEmployeeNumber)
				&& lastEmployeeNumber.Length == 10
				&& int.TryParse(lastEmployeeNumber.Substring(7), out int lastNumber))
			{
				nextNumber = lastNumber + 1;
			}

			// nextNumber 最少 3 位數，不足補零
			return $"{prefix}{nextNumber:D3}";
		}
	}
}
