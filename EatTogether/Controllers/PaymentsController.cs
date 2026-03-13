using EatTogether.Models.Services;
using Microsoft.AspNetCore.Mvc;

namespace EatTogether.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly IOrderService _service;
        public PaymentsController(IOrderService service) => _service = service;

        public async Task<IActionResult> Create()
        {
            var vm = await _service.GetPaymentIndexAsync();
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
        public async Task<IActionResult> Checkout(int preOrderId)
        {
            await _service.CheckoutAsync(preOrderId);
            TempData["Success"] = "結帳成功！";
            return RedirectToAction(nameof(Index));
        }
    }
}
