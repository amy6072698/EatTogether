using EatTogether.Models.DTOs;
using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EatTogether.Controllers
{
    public class TablesController : Controller
    {
        private readonly TableService _tableService;

        public TablesController(TableService tableService)
        {
            _tableService = tableService;
        }

        // GET: /Tables
        public async Task<IActionResult> Index()
        {
            var dtos = await _tableService.GetAllAsync();
            return View(dtos);
        }

        // GET: /Tables/Create
        public IActionResult Create()
        {
            return View(new TableCreateViewModel());
        }

        // POST: /Tables/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TableCreateViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var result = await _tableService.CreateAsync(vm.ToDto());

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(nameof(vm.TableName), result.ErrorMesssage);
                return View(vm);
            }

            TempData["SuccessMessage"] = $"桌位「{vm.TableName}」新增成功";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Tables/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _tableService.GetByIdAsync(id);
            if (dto == null) return NotFound();

            var vm = new TableEditViewModel
            {
                Id = dto.Id,
                TableName = dto.TableName,
                SeatCount = dto.SeatCount,
                Status = dto.Status
            };
            return View(vm);
        }

        // POST: /Tables/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TableEditViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            if (!ModelState.IsValid)
                return View(vm);

            var result = await _tableService.UpdateAsync(new TableDto
            {
                Id = vm.Id,
                TableName = vm.TableName,
                SeatCount = vm.SeatCount
            });

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(nameof(vm.TableName), result.ErrorMesssage);
                return View(vm);
            }

            TempData["SuccessMessage"] = $"桌位「{vm.TableName}」修改成功";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Tables/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tableService.DeleteAsync(id);

            if (!result.IsSuccess)
                TempData["ErrorMessage"] = result.ErrorMesssage;
            else
                TempData["SuccessMessage"] = "桌位已刪除";

            return RedirectToAction(nameof(Index));
        }

        // POST: /Tables/UpdateStatus  (AJAX)
        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] TableUpdateStatusViewModel vm)
        {
            var result = await _tableService.UpdateStatusAsync(vm.Id, vm.Status);
            return Json(new { success = result.IsSuccess, message = result.ErrorMesssage ?? "" });
        }
    }
}
