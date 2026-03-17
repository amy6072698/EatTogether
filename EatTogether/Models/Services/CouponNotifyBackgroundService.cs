using EatTogether.Models.EfModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EatTogether.Models.Services
{
    /// <summary>
    /// 新優惠券上線通知排程服務
    /// 每天 08:00 檢查當天開始的優惠券，發 mail 通知所有有效會員
    /// </summary>
    public class CouponNotifyBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CouponNotifyBackgroundService> _logger;

        public CouponNotifyBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<CouponNotifyBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("新優惠券通知排程服務已啟動");

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                // 下次執行：今天 08:00（若已過則等到明天）
                var nextRun = now.Date.AddHours(8);
                if (now >= nextRun)
                    nextRun = nextRun.AddDays(1);

                var delay = nextRun - now;
                _logger.LogInformation("下次優惠券通知時間：{NextRun}", nextRun.ToString("yyyy/M/d HH:mm"));

                await Task.Delay(delay, stoppingToken);
                if (stoppingToken.IsCancellationRequested) break;

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<EatTogetherDBContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<ReservationEmailService>();

                    var today = DateTime.Today;

                    // 找出今天開始生效的優惠券（StartDate = 今天，且不是生日專屬券）
                    var newCoupons = await context.Coupons
                        .Where(c => c.StartDate.Date == today
                                 && !c.IsDisabled
                                 && !c.Name.Contains("生日"))
                        .ToListAsync(stoppingToken);

                    if (!newCoupons.Any())
                    {
                        _logger.LogInformation("今日（{Date}）無新上線優惠券，略過通知", today.ToString("yyyy/M/d"));
                        continue;
                    }

                    // 取得所有有效會員（有 Email 的）
                    var members = await context.Members
                        .Where(m => !m.IsDeleted && !m.IsBlacklisted
                                 && !string.IsNullOrEmpty(m.Email))
                        .ToListAsync(stoppingToken);

                    int notified = 0;
                    foreach (var coupon in newCoupons)
                    {
                        var discountDesc = coupon.DiscountType == 0
                            ? $"折 ${coupon.DiscountValue}"
                            : $"打 {100 - coupon.DiscountValue} 折";

                        foreach (var member in members)
                        {
                            var body = emailService.BuildCouponNotifyEmail(
                                member.Name,
                                coupon.Name,
                                coupon.Code,
                                discountDesc,
                                coupon.MinSpend,
                                coupon.EndDate);

                            await emailService.EnqueueAsync(
                                member.Email,
                                $"🎉 義起吃新優惠上線！{coupon.Name}，快來領取",
                                body);

                            notified++;
                        }

                        _logger.LogInformation(
                            "優惠券「{Name}」（{Code}）通知已發送給 {Count} 位會員",
                            coupon.Name, coupon.Code, members.Count);
                    }

                    _logger.LogInformation("今日優惠券通知完成，共寄出 {Count} 封", notified);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "優惠券通知排程發生錯誤");
                }
            }
        }
    }
}
