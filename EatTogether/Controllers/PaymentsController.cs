using EatTogether.Models.Services;
using Microsoft.AspNetCore.Mvc;

namespace EatTogether.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly IOrderService _service;
        public PaymentsController(IOrderService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> Create(int? tableId)
        {
            var vm = await _service.GetPaymentIndexAsync();
            ViewBag.DefaultTableId = tableId ?? 0;  // ← 確認有這行
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> GetDetail(int preOrderId)
        {
            var vm = await _service.GetCheckoutDetailAsync(preOrderId);
            if (vm == null) return NotFound();
            return Json(vm);
        }

        [HttpPost]
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
            await _service.CheckoutAsync(preOrderId, payMethod);  // ← 傳入 payMethod
            TempData["Success"] = "結帳成功！";
            return RedirectToAction(nameof(Create));
        }
    }
}
