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

        // GET: /Coupons/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _couponService.GetByIdAsync(id);
            if (dto == null) return NotFound();

            var vm = new CouponEditViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Code = dto.Code,
                DiscountDescription = dto.DiscountDescription,
                ReceivedCount = dto.ReceivedCount,
                LimitCount = dto.LimitCount,
                StatusText = dto.StatusText,
                StatusBadgeClass = dto.StatusBadgeClass,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };
            return View(vm);
        }

        // POST: /Coupons/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CouponEditViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            ModelState.Remove(nameof(vm.Code));
            ModelState.Remove(nameof(vm.DiscountDescription));
            ModelState.Remove(nameof(vm.StatusText));
            ModelState.Remove(nameof(vm.StatusBadgeClass));
            if (!ModelState.IsValid) return View(vm);

            var result = await _couponService.EditAsync(vm.Id, vm.Name, vm.AddLimitCount);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.ErrorMesssage);
                return View(vm);
            }
            TempData["SuccessMessage"] = $"優惠券「{vm.Code}」已更新";
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
