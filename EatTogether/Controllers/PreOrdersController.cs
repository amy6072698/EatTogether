using EatTogether.Models.DTOs;
using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EatTogether.Controllers
{
    public class PreOrdersController : Controller
    {
        private readonly IOrderService _service;
        public PreOrdersController(IOrderService service) => _service = service;

        // Create----------------------------------------------------------------------------
        // 前台：點餐頁
        // GET /PreOrder/Create
        public async Task<IActionResult> Create()
        {
            var vm = new CreatePreOrderViewModel
            {
                TableOptions = await _service.GetTableOptionsAsync(),
                Items = await _service.GetMenuItemsAsync()
            };
            return View(vm);
        }

        // 前台：送出點餐
        // POST /PreOrder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePreOrderViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // 驗證失敗要重新撈選單，否則畫面會空白
                vm.TableOptions = await _service.GetTableOptionsAsync();
                vm.Items = await _service.GetMenuItemsAsync();
                return View(vm);
            }

            var dto = new CreatePreOrderDto
            {
                TableId = vm.TableId,
                PayMethod = "Cash",
                Note = vm.Note,
                Items = vm.Items
                    .Where(i => i.Qty > 0)  // 只送有填數量的
                    .Select(i => new PreOrderDetailDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        Qty = i.Qty,
                        UnitPrice = i.UnitPrice
                    }).ToList()
            };

            await _service.CreatePreOrderAsync(dto);
            TempData["Success"] = "點餐成功！";
            return RedirectToAction(nameof(Create));
        }

        // POST：Create → Confirm
        [HttpPost]
        public async Task<IActionResult> Confirm(CreatePreOrderViewModel vm)
        {
            var confirmVm = new ConfirmPreOrderViewModel
            {
                TableId = vm.TableId,
                InOrOut = vm.InOrOut,
                Note = vm.Note,
                Items = vm.Items.Where(i => i.Qty > 0).ToList()
            };

            if (!confirmVm.Items.Any())
            {
                TempData["Error"] = "請至少選擇一項餐點";
                vm.TableOptions = await _service.GetTableOptionsAsync();
                vm.Items = await _service.GetMenuItemsAsync();
                return View("Create", vm);
            }

            return View(confirmVm);
        }

        // POST：Submit → 存入DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(ConfirmPreOrderViewModel vm)
        {
            var dto = new CreatePreOrderDto
            {
                TableId = vm.TableId,
                InOrOut = vm.InOrOut,
                PayMethod = vm.PayMethod,
                Note = vm.Note,
                DiscountAmount = vm.DiscountAmount,
                Items = vm.Items
                    .Where(i => i.Qty > 0)
                    .Select(i => new PreOrderDetailDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        Qty = i.Qty,
                        UnitPrice = i.UnitPrice
                    }).ToList()
            };

            if (!dto.Items.Any())
            {
                TempData["Error"] = "請至少選擇一項餐點";
                return RedirectToAction(nameof(Create));
            }

            var orderNumber = await _service.CreatePreOrderAsync(dto);
            TempData["Success"] = "點餐成功！";
            TempData["OrderNumber"] = orderNumber;
            return RedirectToAction(nameof(Success));
        }
        // 新增一個 GET 的成功頁
        public IActionResult Success()
        {
            if (TempData["OrderNumber"] == null)
                return RedirectToAction(nameof(Create));

            return View();
        }

        // List------------------------------------------------------------------------------
        public async Task<IActionResult> PreOrderList()
        {
            var vms = await _service.GetPendingPreOrdersAsync();
            return View(vms);
        }

        // AJAX：更新 Detail 狀態
        [HttpPost]
        public async Task<IActionResult> UpdateDetailStatus(int detailId, int status)
        {
            await _service.UpdatePreOrderDetailStatusAsync(detailId, status);
            return Json(new { success = true });
        }
    }
}