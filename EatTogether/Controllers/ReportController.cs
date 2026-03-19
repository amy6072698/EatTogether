using EatTogether.Models.DTOs;
using EatTogether.Models.Infra;
using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EatTogether.Controllers
{
	// [Authorize(Roles = "Admin")]
	[RequirePermission("Report_Manage")]
	public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        public ReportController(IReportService reportService) => _reportService = reportService;

        [HttpGet]
        public async Task<IActionResult> Index(string period = "month",
                                               string? startDate = null,
                                               string? endDate = null)
        {
            var today = DateTime.Today;

            // "all" 要從 DB 撈最早日期，其他直接算
            DateTime start, end;
            if (period == "all")
            {
                // 從 Orders 找最早的 OrderAt
                var earliest = await _reportService.GetEarliestOrderDateAsync();
                start = earliest ?? new DateTime(2024, 1, 1);
                end = today;
            }
            else
            {
                (start, end) = ParseOrDefault(period, startDate, endDate, today);
            }

            var query = new ReportQueryDto { Period = period, StartDate = start, EndDate = end };
            var vm = await _reportService.GetReportAsync(query);

            // 產生各維度下拉選單
            vm.DayOptions = BuildDayOptions(today, start);
            vm.WeekOptions = BuildWeekOptions(today, start);
            vm.MonthOptions = BuildMonthOptions(today, start);
            vm.QuarterOptions = BuildQuarterOptions(today, start);
            vm.YearOptions = BuildYearOptions(today, start);

            return View(vm);
        }

        // ── 解析日期範圍 ─────────────────────────────────────────────────────
        private static (DateTime start, DateTime end) ParseOrDefault(
            string period, string? startDate, string? endDate, DateTime today)
        {
            if (DateTime.TryParse(startDate, out var s) && DateTime.TryParse(endDate, out var e))
                return (s.Date, e.Date);

            return period switch
            {
                "day" => (today, today),
                "week" => GetWeekRange(today),
                "month" => (new DateTime(today.Year, today.Month, 1),
                               new DateTime(today.Year, today.Month,
                                   DateTime.DaysInMonth(today.Year, today.Month))),
                "quarter" => GetQuarterRange(today),
                "year" => (new DateTime(today.Year, 1, 1), new DateTime(today.Year, 12, 31)),
                "custom" => (today.AddDays(-6), today),
                "all" => (new DateTime(2024, 1, 1), today),
                _ => (new DateTime(today.Year, today.Month, 1),
                               new DateTime(today.Year, today.Month,
                                   DateTime.DaysInMonth(today.Year, today.Month)))
            };
        }

        // ── 下拉選單產生器 ────────────────────────────────────────────────────

        // 日：最近 30 天
        private static List<DropdownOption> BuildDayOptions(DateTime today, DateTime selected)
        {
            var list = new List<DropdownOption>();
            for (int i = 0; i < 30; i++)
            {
                var d = today.AddDays(-i);
                var dayNames = new[] { "日", "一", "二", "三", "四", "五", "六" };
                list.Add(new DropdownOption
                {
                    Label = $"{d:yyyy/MM/dd}（週{dayNames[(int)d.DayOfWeek]}）",
                    StartDate = d.ToString("yyyy-MM-dd"),
                    EndDate = d.ToString("yyyy-MM-dd"),
                    IsSelected = d.Date == selected.Date
                });
            }
            return list;
        }

        // 週：最近 16 週，週日～週六
        private static List<DropdownOption> BuildWeekOptions(DateTime today, DateTime selected)
        {
            var list = new List<DropdownOption>();
            // 本週週日
            var thisSunday = today.AddDays(-(int)today.DayOfWeek);
            for (int i = 0; i < 16; i++)
            {
                var sun = thisSunday.AddDays(-7 * i);
                var sat = sun.AddDays(6);
                list.Add(new DropdownOption
                {
                    Label = $"{sun:MM/dd}（日）～ {sat:MM/dd}（六）",
                    StartDate = sun.ToString("yyyy-MM-dd"),
                    EndDate = sat.ToString("yyyy-MM-dd"),
                    IsSelected = selected.Date >= sun.Date && selected.Date <= sat.Date
                });
            }
            return list;
        }

        // 月：最近 12 個月
        private static List<DropdownOption> BuildMonthOptions(DateTime today, DateTime selected)
        {
            var list = new List<DropdownOption>();
            for (int i = 0; i < 12; i++)
            {
                var m = today.AddMonths(-i);
                var first = new DateTime(m.Year, m.Month, 1);
                var last = new DateTime(m.Year, m.Month, DateTime.DaysInMonth(m.Year, m.Month));
                list.Add(new DropdownOption
                {
                    Label = $"{m.Year} 年 {m.Month} 月",
                    StartDate = first.ToString("yyyy-MM-dd"),
                    EndDate = last.ToString("yyyy-MM-dd"),
                    IsSelected = selected.Year == m.Year && selected.Month == m.Month
                });
            }
            return list;
        }

        // 季：最近 8 季
        private static List<DropdownOption> BuildQuarterOptions(DateTime today, DateTime selected)
        {
            var list = new List<DropdownOption>();
            var currentQ = (today.Month - 1) / 3 + 1;
            var qYear = today.Year;
            for (int i = 0; i < 8; i++)
            {
                var q = currentQ - i;
                var y = qYear;
                while (q <= 0) { q += 4; y--; }
                var first = new DateTime(y, (q - 1) * 3 + 1, 1);
                var last = first.AddMonths(3).AddDays(-1);
                list.Add(new DropdownOption
                {
                    Label = $"{y} 年 第 {q} 季",
                    StartDate = first.ToString("yyyy-MM-dd"),
                    EndDate = last.ToString("yyyy-MM-dd"),
                    IsSelected = selected >= first && selected <= last
                });
            }
            return list;
        }

        // 年：最近 5 年
        private static List<DropdownOption> BuildYearOptions(DateTime today, DateTime selected)
        {
            var list = new List<DropdownOption>();
            for (int i = 0; i < 5; i++)
            {
                var y = today.Year - i;
                list.Add(new DropdownOption
                {
                    Label = $"{y} 年",
                    StartDate = $"{y}-01-01",
                    EndDate = $"{y}-12-31",
                    IsSelected = selected.Year == y
                });
            }
            return list;
        }

        private static (DateTime, DateTime) GetWeekRange(DateTime date)
        {
            var sun = date.AddDays(-(int)date.DayOfWeek);
            return (sun, sun.AddDays(6));
        }

        private static (DateTime, DateTime) GetQuarterRange(DateTime date)
        {
            int q = (date.Month - 1) / 3 + 1;
            var start = new DateTime(date.Year, (q - 1) * 3 + 1, 1);
            return (start, start.AddMonths(3).AddDays(-1));
        }
    }
}
