using EatTogether.Models.DTOs;
using EatTogether.Models.Infra;
using EatTogether.Models.Services;
using Microsoft.AspNetCore.Mvc;

namespace EatTogether.Controllers
{
	[RequirePermission("Order_Manage")]
	public class PaymentsController : Controller
    {
        private readonly IOrderService _service;
        public PaymentsController(IOrderService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> Create(int? tableId, bool success = false)
        {
            var vm = await _service.GetPaymentIndexAsync();
            ViewBag.DefaultTableId = tableId ?? 0;
            ViewBag.ShowSuccess = success;  // ← 加這行
            return View(vm);
        }

        [HttpGet]
        [Route("Payments/GetDetail")]
        public async Task<IActionResult> GetDetail(int preOrderId)
        {
            var vm = await _service.GetCheckoutDetailAsync(preOrderId);
            if (vm == null) return NotFound();
            return Json(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelUnserved(int preOrderId)
        {
            await _service.CancelUnservedDetailsAsync(preOrderId);
            var vm = await _service.GetCheckoutDetailAsync(preOrderId);
            return Json(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(int preOrderId, string payMethod)
        {
            await _service.CheckoutAsync(preOrderId, payMethod);
            return RedirectToAction(nameof(Create), new { success = true });
        }
        [HttpGet]
        public async Task<IActionResult> GetDetailByTable(int tableId)
        {
            var vm = await _service.GetCheckoutByTableAsync(tableId);
            if (vm == null) return NotFound();
            return Json(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelUnservedByTable(int tableId)
        {
            await _service.CancelUnservedByTableAsync(tableId);
            var vm = await _service.GetCheckoutByTableAsync(tableId);
            return Json(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckoutByTable(int tableId, string payMethod)
        {
            await _service.CheckoutByTableAsync(tableId, payMethod);
            return RedirectToAction(nameof(Create), new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SplitCheckout([FromBody] SplitCheckoutRequestDto dto)
        {
            // 執行拆單結帳邏輯
            await _service.SplitCheckoutAsync(dto.DetailIds, dto.PayMethod);

            // 重新取得該桌目前「未結帳」的剩餘餐點
            var remaining = await _service.GetCheckoutByTableAsync(dto.TableId);

            // 如果 remaining 為 null，代表該訂單已完全結清
            return Json(new
            {
                success = true,
                remaining = remaining,
                isFullyPaid = (remaining == null || remaining.Items.Count == 0)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderTable(int preOrderId, int? tableId, bool inOrOut)
        {
            await _service.UpdateOrderTableAsync(preOrderId, tableId, inOrOut);
            var vm = await _service.GetCheckoutDetailAsync(preOrderId);
            return Json(vm);
        }

        [HttpGet]
        public async Task<IActionResult> AvailableTables()
        {
            var vm = await _service.GetPaymentIndexAsync();
            var tables = vm.Tables
                .Where(t => !t.HasOrder && !t.IsOccupied)
                .Select(t => new { t.TableId, t.TableName })
                .ToList();
            return Json(tables);
        }

        [HttpGet]
        public async Task<IActionResult> GetManualEvents(int? tableId, int? preOrderId)
        {
            var events = await _service.GetManualEventsForOrderAsync(tableId, preOrderId);
            return Json(events);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyEvent(int? tableId, int? preOrderId, int? eventId)
        {
            try
            {
                var vm = await _service.ApplyEventToOrderAsync(tableId, preOrderId, eventId);
                if (vm == null) return Json(new { success = false, error = "找不到訂單" });
                return Json(new { success = true, data = vm });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = $"套用失敗：{ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyCoupon(int? tableId, int? preOrderId, string couponCode)
        {
            var (success, error, vm) = await _service.ApplyCouponToOrderAsync(tableId, preOrderId, couponCode);
            if (!success) return Json(new { success = false, error });
            return Json(new { success = true, data = vm });
        }

        [HttpGet]
        public async Task<IActionResult> GetApplicableCoupons(int? tableId, int? preOrderId)
        {
            var coupons = await _service.GetApplicableCouponsForOrderAsync(tableId, preOrderId);
            return Json(coupons);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyCouponById(int? tableId, int? preOrderId, int? couponId)
        {
            try
            {
                var vm = await _service.ApplyCouponByIdToOrderAsync(tableId, preOrderId, couponId);
                if (vm == null) return Json(new { success = false, error = "找不到訂單" });
                return Json(new { success = true, data = vm });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = $"套用失敗：{ex.Message}" });
            }
        }
    }
}
