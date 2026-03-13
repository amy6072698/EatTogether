using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EatTogether.Controllers
{
    public class CouponsController : Controller
    {
        private readonly CouponService _couponService;

        public CouponsController(CouponService couponService)
        {
            _couponService = couponService;
        }

        // GET: /Coupons
        public async Task<IActionResult> Index()
        {
            var dtos = await _couponService.GetAllAsync();
            return View(dtos);
        }

        // GET: /Coupons/Create
        public IActionResult Create()
        {
            return View(new CouponCreateViewModel());
        }

        // POST: /Coupons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponCreateViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var result = await _couponService.CreateAsync(vm.ToDto());

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(nameof(vm.Code), result.ErrorMesssage);
                return View(vm);
            }

            TempData["SuccessMessage"] = $"優惠券「{vm.Code}」建立成功";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Coupons/MemberCoupons  (後台領券紀錄)
        public async Task<IActionResult> MemberCoupons()
        {
            var dtos = await _couponService.GetAllMemberCouponsAsync();
            return View(dtos);
        }

        // POST: /Coupons/ApplyCoupon  (AJAX，供結帳頁呼叫)
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponRequest req)
        {
            var (result, discount) = await _couponService.RedeemCouponAsync(
                req.Code, req.MemberId, req.OrderAmount);

            return Json(new
            {
                success = result.IsSuccess,
                discountAmount = discount,
                message = result.ErrorMesssage ?? ""
            });
        }
    }

    public class ApplyCouponRequest
    {
        public string Code { get; set; } = null!;
        public int MemberId { get; set; }
        public int OrderAmount { get; set; }
    }
}
