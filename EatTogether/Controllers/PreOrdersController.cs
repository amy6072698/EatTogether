using EatTogether.Models.DTOs;
using EatTogether.Models.Infra;
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
		[RequirePermission("Order_StatusUpdate")]
		public async Task<IActionResult> Create(int? tableId)
        {
            var vm = new CreatePreOrderViewModel
            {
                TableId = tableId ?? 0,
                TableOptions = await _service.GetTableOptionsAsync(tableId),  // ← 傳入 tableId
                Items = await _service.GetMenuItemsAsync()
            };
            return View(vm);
        }

		// 前台：送出點餐
		// POST /PreOrder/Create
		[RequirePermission("Order_StatusUpdate")]
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
		[RequirePermission("Order_StatusUpdate")]
		[HttpPost]
        public async Task<IActionResult> Confirm(CreatePreOrderViewModel vm)
        {
            // 先取出有數量的主項目，記錄舊 index → 新 index 的對應
            var parentItems = vm.Items
                .Select((item, oldIdx) => new { item, oldIdx })
                .Where(x => x.item.Qty > 0 && !x.item.ParentIndex.HasValue)
                .ToList();

            var indexMap = parentItems
                .Select((x, newIdx) => new { x.oldIdx, newIdx })
                .ToDictionary(x => x.oldIdx, x => x.newIdx);

            // 取出子項目，更新 ParentIndex
            var childItems = vm.Items
                .Where(i => i.ParentIndex.HasValue && indexMap.ContainsKey(i.ParentIndex.Value))
                .Select(i => {
                    i.ParentIndex = indexMap[i.ParentIndex.Value];
                    return i;
                })
                .ToList();

            var allItems = parentItems.Select(x => x.item).ToList();
            allItems.AddRange(childItems);

            var confirmVm = new ConfirmPreOrderViewModel
            {
                TableId = vm.TableId,
                InOrOut = vm.InOrOut,
                Note = vm.Note,
                Items = allItems
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
		[RequirePermission("Order_StatusUpdate")]
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
                CouponId = vm.CouponId,
                DiscountAmount = vm.DiscountAmount,
                Items = vm.Items
                    .Where(i => i.Qty > 0 || i.ParentIndex.HasValue)
                    .Select(i => new PreOrderDetailDto
                    {
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        Qty = i.Qty,
                        UnitPrice = i.UnitPrice,
                        IsSetMeal = i.IsSetMeal,
                        ParentIndex = i.ParentIndex
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
		[RequirePermission("Order_StatusUpdate")]
		public IActionResult Success()
        {
            if (TempData["OrderNumber"] == null)
                return RedirectToAction(nameof(Create));

            return View();
        }

		// List------------------------------------------------------------------------------
		[RequirePermission("Order_StatusUpdate")]
		public async Task<IActionResult> TodayPreOrderList()
        {
            var vms = await _service.GetPendingPreOrdersAsync();
            return View(vms);
        }

		[RequirePermission("Order_StatusUpdate")]
		[HttpGet]
        public async Task<IActionResult> PendingCount()
        {
            var list = await _service.GetPendingPreOrdersAsync();
            return Json(new { count = list.Count });
        }

		[RequirePermission("Order_Manage")]
		public async Task<IActionResult> AllOrders(PreOrderListQueryViewModel query)
        {
            if (query.Page < 1) query.Page = 1;
            var vm = await _service.GetAllPreOrdersAsync(query);
            return View(vm);
        }

		// AJAX：更新 Detail 狀態
		[RequirePermission("Order_StatusUpdate")]
		[HttpPost]
        public async Task<IActionResult> UpdateDetailStatus(int detailId, int status)
        {
            await _service.UpdatePreOrderDetailStatusAsync(detailId, status);
            return Json(new { success = true });
        }

		// AJAX: 整單取消
		[RequirePermission("Order_Manage")]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int preOrderId)
        {
            await _service.CancelOrderAsync(preOrderId);
            return Json(new { success = true });
        }

		[RequirePermission("Order_Manage")]
		public async Task<IActionResult> Detail(int id)
        {
            var vm = await _service.GetPreOrderDetailAsync(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

		[RequirePermission("Order_StatusUpdate")]
		[HttpGet]
        public async Task<IActionResult> ValidateCoupon(string code, int originalAmount)
        {
            var result = await _service.ValidateCouponAsync(code, originalAmount);
            return Json(result);
        }

		[RequirePermission("Order_Manage")]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelAllByTable(int tableId)
        {
            await _service.CancelAllByTableAsync(tableId);
            return Json(new { success = true });
        }

		[RequirePermission("Order_StatusUpdate")]
		[HttpGet]
        public async Task<IActionResult> GetSetMealItems(int setMealId)
        {
            var groups = await _service.GetSetMealItemsAsync(setMealId);
            return Json(groups);
        }
    }
}