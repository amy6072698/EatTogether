using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EatTogether.Models.Services
{
    /// <summary>
    /// 生日優惠券排程服務
    /// 每月1號 00:05 統一發放當月共用生日優惠券
    /// </summary>
    public class BirthdayCouponBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BirthdayCouponBackgroundService> _logger;

        public BirthdayCouponBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<BirthdayCouponBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("生日優惠券排程服務已啟動");

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;

                // 計算下次執行時間：下個月1號 00:05
                var nextMonth = new DateTime(now.Year, now.Month, 1).AddMonths(1);
                var nextRun = nextMonth.AddMinutes(5);
                var delay = nextRun - now;

                _logger.LogInformation(
                    "下次生日券發放時間：{NextRun}",
                    nextRun.ToString("yyyy/M/d HH:mm"));

                await Task.Delay(delay, stoppingToken);

                if (stoppingToken.IsCancellationRequested) break;

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var service = scope.ServiceProvider
                                          .GetRequiredService<BirthdayCouponService>();
                    var issued = await service.IssueBirthdayCouponsAsync();
                    _logger.LogInformation(
                        "生日優惠券發放完成，{Year}年{Month}月共發放 {Count} 位壽星",
                        DateTime.Today.Year, DateTime.Today.Month, issued);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "生日優惠券排程發生錯誤");
                }
            }
        }
    }
}
