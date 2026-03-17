using EatTogether.Models.EfModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace EatTogether.Models.Services
{
    public class ReservationEmailService
    {
        private readonly EatTogetherDBContext _context;
        private readonly IConfiguration _config;

        public ReservationEmailService(EatTogetherDBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        /// <summary>寄出郵件並寫入 EmailQueue 備查</summary>
        public async Task EnqueueAsync(string recipientEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(recipientEmail)) return;

            // 1. 寫入 EmailQueue 備查
            _context.EmailQueues.Add(new EmailQueue
            {
                RecipientEmail = recipientEmail,
                Subject = subject,
                Body = body,
                Status = 0,
                CreatedAt = DateTime.Now
            });
            await _context.SaveChangesAsync();

            // 2. 透過 SMTP 實際寄出
            try
            {
                var smtp = _config.GetSection("Smtp");
                var host = smtp["Host"] ?? "smtp.gmail.com";
                var port = int.Parse(smtp["Port"] ?? "587");
                var enableSsl = bool.Parse(smtp["EnableSsl"] ?? "true");
                var userName = smtp["UserName"] ?? "";
                var password = smtp["Password"] ?? "";
                var fromName = smtp["FromName"] ?? "義起吃後台系統";
                var fromAddress = smtp["FromAddress"] ?? userName;

                using var client = new SmtpClient(host, port)
                {
                    EnableSsl = enableSsl,
                    Credentials = new NetworkCredential(userName, password)
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(fromAddress, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mail.To.Add(recipientEmail);

                await client.SendMailAsync(mail);

                // 更新 EmailQueue 狀態為已發送
                var queue = await _context.EmailQueues
                    .OrderByDescending(q => q.Id)
                    .FirstOrDefaultAsync(q => q.RecipientEmail == recipientEmail
                                           && q.Subject == subject);
                if (queue != null)
                {
                    queue.Status = 1; // 1=已發送
                    queue.ProcessedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // 寄信失敗不中斷主流程，記錄失敗狀態
                var queue = await _context.EmailQueues
                    .OrderByDescending(q => q.Id)
                    .FirstOrDefaultAsync(q => q.RecipientEmail == recipientEmail
                                           && q.Subject == subject);
                if (queue != null)
                {
                    queue.Status = 2; // 2=發送失敗
                    await _context.SaveChangesAsync();
                }
                // 可視需要加 logging：_logger.LogError(ex, "寄信失敗");
                _ = ex; // suppress unused warning
            }
        }

        /// <summary>訂位確認信內容</summary>
        public string BuildReservationEmail(string name, string bookingNumber,
            DateTime reservationDate, int adults, int children, string? remark)
        {
            var remarkLine = string.IsNullOrEmpty(remark) ? "" :
                $"<tr><td style='padding:6px 0;color:#666;'>備註</td>" +
                $"<td style='padding:6px 0;'>{remark}</td></tr>";

            return $@"
<div style='font-family:sans-serif;max-width:560px;margin:auto;'>
  <div style='background:#1a1a2e;padding:24px;border-radius:8px 8px 0 0;text-align:center;'>
    <h2 style='color:#fff;margin:0;'>義起吃 Eat Together</h2>
    <p style='color:#ccc;margin:4px 0 0;'>訂位確認通知</p>
  </div>
  <div style='background:#fff;padding:28px;border:1px solid #eee;border-top:none;border-radius:0 0 8px 8px;'>
    <p>親愛的 <strong>{name}</strong> 您好，</p>
    <p>您的訂位已確認！以下是您的訂位資訊：</p>
    <table style='width:100%;border-collapse:collapse;margin:16px 0;'>
      <tr><td style='padding:6px 0;color:#666;'>訂位代號</td>
          <td style='padding:6px 0;'><code style='background:#f5f5f5;padding:2px 8px;border-radius:4px;'>{bookingNumber}</code></td></tr>
      <tr><td style='padding:6px 0;color:#666;'>預約時間</td>
          <td style='padding:6px 0;'><strong>{reservationDate:yyyy年M月d日 HH:mm}</strong></td></tr>
      <tr><td style='padding:6px 0;color:#666;'>用餐人數</td>
          <td style='padding:6px 0;'>{adults} 大人{(children > 0 ? $" + {children} 小孩" : "")}</td></tr>
      {remarkLine}
    </table>
    <p style='color:#666;font-size:0.9em;'>如需更改或取消訂位，請提前來電告知。感謝您選擇義起吃！</p>
    <hr style='border:none;border-top:1px solid #eee;margin:20px 0;'/>
    <p style='color:#999;font-size:0.8em;text-align:center;'>義起吃餐廳 &nbsp;|&nbsp; 本郵件為系統自動發送，請勿直接回覆</p>
  </div>
</div>";
        }

        /// <summary>生日優惠券通知信內容</summary>
        public string BuildBirthdayCouponEmail(string name, string couponCode,
            int discountValue, DateTime endDate)
        {
            return $@"
<div style='font-family:sans-serif;max-width:560px;margin:auto;'>
  <div style='background:#c0392b;padding:24px;border-radius:8px 8px 0 0;text-align:center;'>
    <h2 style='color:#fff;margin:0;'>🎂 生日快樂！</h2>
    <p style='color:#fdd;margin:4px 0 0;'>義起吃 Eat Together</p>
  </div>
  <div style='background:#fff;padding:28px;border:1px solid #eee;border-top:none;border-radius:0 0 8px 8px;'>
    <p>親愛的 <strong>{name}</strong>，</p>
    <p>祝您生日快樂！🎉 義起吃特別為您準備了一份生日禮物：</p>
    <div style='background:#fff8e1;border:2px dashed #f0a500;border-radius:8px;padding:20px;text-align:center;margin:20px 0;'>
      <div style='font-size:0.9em;color:#666;margin-bottom:8px;'>您的生日專屬折扣碼</div>
      <div style='font-size:2em;font-weight:bold;letter-spacing:4px;color:#c0392b;'>{couponCode}</div>
      <div style='margin-top:8px;color:#888;font-size:0.9em;'>折抵 ${discountValue} 元 | 限 {endDate:yyyy/M/d} 前使用</div>
    </div>
    <p style='color:#666;font-size:0.9em;'>結帳時輸入折扣碼即可享受折扣，祝您用餐愉快！</p>
    <hr style='border:none;border-top:1px solid #eee;margin:20px 0;'/>
    <p style='color:#999;font-size:0.8em;text-align:center;'>義起吃餐廳 &nbsp;|&nbsp; 本郵件為系統自動發送，請勿直接回覆</p>
  </div>
</div>";
        }
        /// <summary>新優惠券上線通知信內容</summary>
        public string BuildCouponNotifyEmail(string memberName, string couponName,
            string couponCode, string discountDesc, int minSpend, DateTime? endDate)
        {
            var validLine = endDate.HasValue
                ? $"<div style='margin-top:8px;color:#888;font-size:0.9em;'>限 {endDate.Value:yyyy/M/d} 前使用</div>"
                : "<div style='margin-top:8px;color:#888;font-size:0.9em;'>永久有效</div>";
            var minSpendLine = minSpend > 0
                ? $"<div style='color:#888;font-size:0.9em;'>最低消費 ${minSpend}</div>"
                : "";

            return $@"
<div style='font-family:sans-serif;max-width:560px;margin:auto;'>
  <div style='background:#2c3e50;padding:24px;border-radius:8px 8px 0 0;text-align:center;'>
    <h2 style='color:#fff;margin:0;'>🎉 新優惠券上線！</h2>
    <p style='color:#ccc;margin:4px 0 0;'>義起吃 Eat Together</p>
  </div>
  <div style='background:#fff;padding:28px;border:1px solid #eee;border-top:none;border-radius:0 0 8px 8px;'>
    <p>親愛的 <strong>{memberName}</strong>，</p>
    <p>我們有新的優惠券上線了，趕快來領取吧！</p>
    <div style='background:#f0f9ff;border:2px solid #3498db;border-radius:8px;padding:20px;text-align:center;margin:20px 0;'>
      <div style='font-size:1em;color:#2980b9;font-weight:bold;margin-bottom:8px;'>{couponName}</div>
      <div style='font-size:1.8em;font-weight:bold;letter-spacing:3px;color:#e74c3c;'>{couponCode}</div>
      <div style='margin-top:8px;color:#555;font-size:1em;font-weight:bold;'>{discountDesc}</div>
      {minSpendLine}
      {validLine}
    </div>
    <p style='color:#666;font-size:0.9em;'>結帳時輸入折扣碼即可享受折扣，數量有限，先搶先贏！</p>
    <hr style='border:none;border-top:1px solid #eee;margin:20px 0;'/>
    <p style='color:#999;font-size:0.8em;text-align:center;'>義起吃餐廳 &nbsp;|&nbsp; 本郵件為系統自動發送，請勿直接回覆</p>
  </div>
</div>";
        }
        /// <summary>只做 SMTP 發送，不寫 EmailQueue（供背景任務使用）</summary>
        public async Task SendOnlyAsync(string recipientEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(recipientEmail)) return;
            try
            {
                var smtp = _config.GetSection("Smtp");
                var host = smtp["Host"] ?? "smtp.gmail.com";
                var port = int.Parse(smtp["Port"] ?? "587");
                var enableSsl = bool.Parse(smtp["EnableSsl"] ?? "true");
                var userName = smtp["UserName"] ?? "";
                var password = smtp["Password"] ?? "";
                var fromName = smtp["FromName"] ?? "義起吃後台系統";
                var fromAddress = smtp["FromAddress"] ?? userName;

                using var client = new SmtpClient(host, port)
                {
                    EnableSsl = enableSsl,
                    Credentials = new NetworkCredential(userName, password)
                };
                var mail = new MailMessage
                {
                    From = new MailAddress(fromAddress, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mail.To.Add(recipientEmail);
                await client.SendMailAsync(mail);
            }
            catch { /* 發送失敗靜默處理 */ }
        }
    }
}