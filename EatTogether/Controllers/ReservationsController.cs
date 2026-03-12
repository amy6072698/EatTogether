using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EatTogether.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ReservationService _reservationService;

        public ReservationsController(ReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        // GET: /Reservations?searchDate=yyyy-MM-dd&statusFilter=0
        public async Task<IActionResult> Index(DateTime? searchDate, int? statusFilter)
        {
            var vm = new ReservationSearchViewModel
            {
                SearchDate = searchDate,
                StatusFilter = statusFilter
            };

            var all = searchDate.HasValue
                ? await _reservationService.GetByDateAsync(searchDate.Value)
                : await _reservationService.GetAllAsync();

            // 狀態篩選
            if (statusFilter.HasValue)
                all = all.Where(r => r.Status == statusFilter.Value);

            vm.Results = all;
            return View(vm);
        }

        // GET: /Reservations/Create
        public IActionResult Create()
        {
            return View(new ReservationCreateViewModel());
        }

        // POST: /Reservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationCreateViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var result = await _reservationService.CreateAsync(vm.ToDto());

            if (!result.IsSuccess)
            {
                ViewBag.ErrorMessage = result.ErrorMesssage;
                return View(vm);
            }

            TempData["SuccessMessage"] = "訂位新增成功";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Reservations/UpdateStatus  (AJAX)
        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] ReservationUpdateStatusViewModel vm)
        {
            var result = await _reservationService.UpdateStatusAsync(vm.Id, vm.Status);
            return Json(new { success = result.IsSuccess, message = result.ErrorMesssage ?? "" });
        }
    }
}
