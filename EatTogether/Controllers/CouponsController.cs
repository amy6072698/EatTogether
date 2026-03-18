using EatTogether.Models.Services;
using System;
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
                ModelState.AddModelError(nameof(vm.Code), result.ErrorMessage);
                return View(vm);
            }

            var now = DateTime.Now;
            bool notified = vm.StartDate <= now && (!vm.EndDate.HasValue || vm.EndDate.Value >= now);
            TempData["SuccessMessage"] = notified
                ? $"優惠券「{vm.Code}」建立成功，已發送通知給所有會員！"
                : $"優惠券「{vm.Code}」建立成功";
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
                EndDate = dto.EndDate,
                IsDisabled = dto.IsDisabled
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

            var result = await _couponService.EditAsync(vm.Id, vm.Name, vm.AddLimitCount, vm.NewEndDate);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.ErrorMessage);
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

        // POST: /Coupons/IssueToAll/5（一鍵發放給全會員）
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IssueToAll(int id)
        {
            var (issued, skipped) = await _couponService.IssueToAllMembersAsync(id);
            TempData["SuccessMessage"] = $"發放完成！新增 {issued} 筆，跳過（已領） {skipped} 筆";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Coupons/Disable/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable(int id)
        {
            var result = await _couponService.DisableAsync(id);
            TempData["SuccessMessage"] = result.IsSuccess ? "優惠券已停用" : result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        // POST: /Coupons/Enable/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enable(int id)
        {
            var result = await _couponService.EnableAsync(id);
            TempData["SuccessMessage"] = result.IsSuccess ? "優惠券已重新啟用" : result.ErrorMessage;
            return RedirectToAction(nameof(Index));
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
                message = result.ErrorMessage ?? ""
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
