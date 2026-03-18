using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.Repositories;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EatTogether.Models.Services
{
    public interface IOrderService
    {
        // CreatePreOrder
        Task<string> CreatePreOrderAsync(CreatePreOrderDto dto);
        Task<List<SelectListItem>> GetTableOptionsAsync(int? includeTableId = null);
        Task<List<CreatePreOrderItemViewModel>> GetMenuItemsAsync();
        Task<CouponValidateDto> ValidateCouponAsync(string code, int originalAmount);
        Task CancelAllByTableAsync(int tableId);
        Task<List<SetMealItemGroupDto>> GetSetMealItemsAsync(int setMealId);

        // PreOrdersList
        Task<List<PreOrderListItemViewModel>> GetPendingPreOrdersAsync();
        Task UpdatePreOrderDetailStatusAsync(int detailId, int status);
        Task<PreOrderListQueryViewModel> GetAllPreOrdersAsync(PreOrderListQueryViewModel query);
        Task CancelOrderAsync(int preOrderId);

        // Details
        Task<PreOrderListItemViewModel> GetPreOrderDetailAsync(int preOrderId);

        // Payment
        Task<PaymentCheckoutViewModel> GetCheckoutDetailAsync(int preOrderId);
        Task CancelUnservedDetailsAsync(int preOrderId);
        Task<int> CheckoutAsync(int preOrderId, string payMethod);
        Task<PaymentIndexViewModel> GetPaymentIndexAsync();
        Task<PaymentCheckoutViewModel?> GetCheckoutByTableAsync(int tableId);
        Task CancelUnservedByTableAsync(int tableId);
        Task<int> CheckoutByTableAsync(int tableId, string payMethod);
        Task<int> SplitCheckoutAsync(List<int> detailIds, string payMethod);
        Task UpdateOrderTableAsync(int preOrderId, int? newTableId, bool inOrOut);
    }
    public class OrderService : IOrderService
    {
        private readonly IPreOrderRepository _preOrderRepo;
        private readonly ITableRepository _tableRepo;
        private readonly IProductRepository _productRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly ICouponRepository _couponRepo;

        public OrderService(
            IPreOrderRepository preOrderRepo,
            ITableRepository tableRepo,
            IProductRepository productRepo,
            IOrderRepository orderRepo,
            ICouponRepository couponRepo)
        {
            _preOrderRepo = preOrderRepo;
            _tableRepo = tableRepo;
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _couponRepo = couponRepo;
        }

        // ── CreatePreOrder ──────────────────────────────────────────────────
        public async Task<string> CreatePreOrderAsync(CreatePreOrderDto dto)
        {
            var orderNumber = await GenerateOrderNumberAsync();
            var originalAmount = dto.Items
                .Where(i => !i.ParentIndex.HasValue) // 只算主項目金額（套餐標題 + 單品），子項目不計
                .Sum(i => i.Qty * i.UnitPrice);
            var discountAmount = dto.DiscountAmount;

            // 第一階段：建立 detail 列表
            var details = dto.Items.Select(i => new PreOrderDetail
            {
                ProductId = i.ProductId > 0 ? i.ProductId : 1,
                ProductName = i.ProductName,
                Qty = i.Qty,
                UnitPrice = (int)i.UnitPrice,
                SubTotal = i.ParentIndex.HasValue ? 0 : (int)(i.Qty * i.UnitPrice),
                IsSetMeal = i.IsSetMeal,
                DoneOrCancel = 0
            }).ToList();

            var preOrder = new PreOrder
            {
                OrderNumber = orderNumber,
                InOrOut = dto.InOrOut,
                TableId = dto.InOrOut ? dto.TableId : null,
                OrderAt = DateTime.Now,
                OriginalAmount = (int)originalAmount,
                CouponId = dto.CouponId,
                DiscountAmount = discountAmount,
                TotalAmount = (int)(originalAmount - discountAmount),
                Note = dto.Note,
                PayMethod = dto.PayMethod,
                DoneOrCancel = PreOrderStatus.Pending,
                PreOrderDetails = details
            };

            await _preOrderRepo.AddAsync(preOrder); // 這裡 SaveChanges，details 的 Id 有值了

            // 第二階段：設定子項目的 ParentDetailId
            bool hasChildren = false;
            for (int i = 0; i < dto.Items.Count; i++)
            {
                if (dto.Items[i].ParentIndex.HasValue)
                {
                    var parentIdx = dto.Items[i].ParentIndex.Value;
                    // 加這行看看
                    var parentDetail = details.ElementAtOrDefault(parentIdx);
                    // parentDetail 是不是 null？parentIdx 有沒有超出範圍？
                    details[i].ParentDetailId = details[parentIdx].Id;
                    hasChildren = true;
                }
            }

            if (hasChildren)
                await _preOrderRepo.SaveChangesAsync();

            return orderNumber;
        }

        // 訂單編號產生器
        private async Task<string> GenerateOrderNumberAsync()
        {
            var today = DateTime.Today;
            var count = await _preOrderRepo.CountTodayAsync(today);
            return today.ToString("yyyyMMdd") + "-" + (count + 1).ToString("D4");
            // D4 = 補零到4位，例如 0001, 0010
        }

        public async Task<List<SelectListItem>> GetTableOptionsAsync(int? includeTableId = null)
        {
            var tables = await _tableRepo.GetAllAsync();
            var today = DateTime.Today;
            var pendingOrders = await _preOrderRepo.GetByStatusAsync(PreOrderStatus.Pending);
            var occupiedTableIds = pendingOrders
                .Where(p => p.InOrOut && p.OrderAt.Date == today && p.TableId.HasValue)
                .Select(p => p.TableId!.Value)
                .ToHashSet();

            return tables
                .Where(t => t.Status == 1)
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.TableName,
                    Selected = t.Id == includeTableId
                }).ToList();
        }

        public async Task<List<CreatePreOrderItemViewModel>> GetMenuItemsAsync()
        {
            var products = await _productRepo.GetAllAsync();
            var result = new List<CreatePreOrderItemViewModel>();

            foreach (var p in products)
            {
                // 根據 ProductType 取得名稱與價格
                string? name = p.ProductType == "Dish" ? p.DishName : p.SetMealName;
                decimal? price = p.ProductType == "Dish" ? p.DishPrice : p.SetMealPrice;

                if (string.IsNullOrEmpty(name)) continue;

                result.Add(new CreatePreOrderItemViewModel
                {
                    ProductId = p.Id,
                    ProductName = name,
                    UnitPrice = (int)(price ?? 0),
                    Qty = 0,
                    IsSetMeal = p.ProductType == "SetMeal",
                    CategoryName = p.ProductType == "Dish" ? p.DishCategoryName : null
                });
            }
            return result;
        }

        public async Task<CouponValidateDto> ValidateCouponAsync(string code, int originalAmount)
        {
            var coupon = await _couponRepo.GetByCodeAsync(code);

            if (coupon == null)
                return new CouponValidateDto { IsValid = false, Message = "折扣碼無效或已過期" };

            if (originalAmount < coupon.MinSpend)
                return new CouponValidateDto
                {
                    IsValid = false,
                    Message = $"未達最低消費 NT$ {coupon.MinSpend}"
                };

            int discount = coupon.DiscountType == 0
                ? (int)coupon.DiscountValue
                : (int)(originalAmount * coupon.DiscountValue / 100);

            discount = Math.Min(discount, originalAmount);

            return new CouponValidateDto
            {
                IsValid = true,
                CouponId = coupon.Id,
                CouponName = coupon.Name,
                Discount = discount,
                Message = coupon.DiscountType == 0
                    ? $"✅ 折抵 NT$ {discount} 已套用！"
                    : $"✅ 打 {(100 - coupon.DiscountValue) / 10.0:0.#} 折已套用！"
            };
        }

        public async Task CancelAllByTableAsync(int tableId)
        {
            await _preOrderRepo.CancelAllByTableIdAsync(tableId);
            await _tableRepo.UpdateStatusAsync(tableId, 0);
        }

        public async Task<List<SetMealItemGroupDto>> GetSetMealItemsAsync(int setMealId) => 
            await _productRepo.GetSetMealItemsAsync(setMealId);

        // ── PreOrdersList ──────────────────────────────────────────────────
        public async Task<List<PreOrderListItemViewModel>> GetPendingPreOrdersAsync()
        {
            var list = await _preOrderRepo.GetByStatusAsync(PreOrderStatus.Pending);
            var today = DateTime.Today;

            return list
                .Where(p => p.OrderAt.Date == today)   // ← 只顯示當日
                .Where(p => p.PreOrderDetails.Any(d => d.DoneOrCancel == 0))
                .OrderBy(p => p.OrderAt)
                .Select(p => new PreOrderListItemViewModel
                {
                    PreOrderId = p.Id,
                    OrderNumber = p.OrderNumber,
                    InOrOut = p.InOrOut,
                    TableName = p.Table?.TableName ?? "外帶",
                    OrderAt = p.OrderAt,
                    Note = p.Note,
                    PendingCount = p.PreOrderDetails.Count(d => d.DoneOrCancel == 0),
                    Items = p.PreOrderDetails.Select(d => new PreOrderDetailItemViewModel
                    {
                        DetailId = d.Id,
                        PreOrderId = p.Id,
                        ProductName = d.ProductName,
                        Qty = d.Qty,
                        Status = d.DoneOrCancel,
                        IsSetMeal = d.IsSetMeal,
                        ParentDetailId = d.ParentDetailId
                    }).ToList()
                }).ToList();
        }

        public async Task UpdatePreOrderDetailStatusAsync(int detailId, int status)
        {
            await _preOrderRepo.UpdateDetailStatusAsync(detailId, status);

            // 取得該 detail 的 PreOrderId
            var preOrderId = await _preOrderRepo.GetPreOrderIdByDetailIdAsync(detailId);

            // 檢查是否所有餐點都取消了
            var preOrder = await _preOrderRepo.GetByIdAsync(preOrderId);
            if (preOrder != null && preOrder.DoneOrCancel == PreOrderStatus.Pending
                && preOrder.PreOrderDetails.All(d => d.DoneOrCancel == 2))
            {
                preOrder.CancelledAt = DateTime.Now;
                await _preOrderRepo.UpdateStatusAsync(preOrderId, PreOrderStatus.Cancel);
            }
        }

        public async Task<PreOrderListQueryViewModel> GetAllPreOrdersAsync(PreOrderListQueryViewModel query)
        {
            var all = await _preOrderRepo.GetAllAsync();  // 需補 GetAllAsync

            // 篩選
            var filtered = all.AsQueryable();

            if (query.Status.HasValue)
                filtered = filtered.Where(p => p.DoneOrCancel == query.Status.Value);

            if (query.DateFrom.HasValue)
                filtered = filtered.Where(p => p.OrderAt.Date >= query.DateFrom.Value.Date);

            if (query.DateTo.HasValue)
                filtered = filtered.Where(p => p.OrderAt.Date <= query.DateTo.Value.Date);

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                var kw = query.Keyword.Trim();
                filtered = filtered.Where(p =>
                    p.OrderNumber.Contains(kw) ||
                    (p.Member != null && p.Member.Name.Contains(kw)) ||
                    (p.Table != null && p.Table.TableName.Contains(kw)) ||
                    (p.PayMethod != null && p.PayMethod.Contains(kw))
                );
            }

            // 計算總筆數
            query.TotalCount = filtered.Count();

            // 分頁
            query.Orders = filtered
                .OrderByDescending(p => p.OrderNumber)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new PreOrderListItemViewModel
                {
                    MemberName = p.Member != null ? MaskName(p.Member.Name) : "訪客",
                    PreOrderId = p.Id,
                    OrderNumber = p.OrderNumber,
                    InOrOut = p.InOrOut,
                    TableName = p.Table != null ? p.Table.TableName : "外帶",
                    OrderAt = p.OrderAt,
                    OriginalAmount = p.OriginalAmount,
                    DiscountAmount = p.DiscountAmount,
                    TotalAmount = p.TotalAmount,
                    DoneOrCancel = p.DoneOrCancel,
                    PayMethod = p.PayMethod,
                    CouponName = p.Coupon != null ? p.Coupon.Name : null,
                    CouponDesc = p.Coupon != null
                                 ? (p.Coupon.DiscountType == 0
                                 ? $"折抵 NT$ {p.Coupon.DiscountValue}"
                                 : $"打 {(100 - p.Coupon.DiscountValue) / 10.0:0.#} 折")
                                 : null
                }).ToList();

            return query;
        }

        private string MaskName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length < 2) return name;
            return name[0] + "*" + name[2..];  // 第2個字換成 *
        }
        public async Task CancelOrderAsync(int preOrderId)
        {
            await _preOrderRepo.CancelEntireOrderAsync(preOrderId);

            // 如果是內用，把桌位改回空桌
            var preOrder = await _preOrderRepo.GetByIdAsync(preOrderId);
            if (preOrder?.TableId.HasValue == true)
                await _tableRepo.UpdateStatusAsync(preOrder.TableId.Value, 0);
        }

        // ── Details ──────────────────────────────────────────────────
        public async Task<PreOrderListItemViewModel> GetPreOrderDetailAsync(int preOrderId)
        {
            var p = await _preOrderRepo.GetByIdAsync(preOrderId);
            if (p == null) return null;

            // 優惠券描述
            string couponName = null;
            string couponDesc = null;
            if (p.Coupon != null)
            {
                couponName = p.Coupon.Name;
                couponDesc = p.Coupon.DiscountType == 0
                    ? $"折抵 NT$ {p.Coupon.DiscountValue}"
                    : $"打 {(100 - p.Coupon.DiscountValue) / 10.0:0.#} 折";
            }

            return new PreOrderListItemViewModel
            {
                PreOrderId = p.Id,
                OrderNumber = p.OrderNumber,
                InOrOut = p.InOrOut,
                TableName = p.Table?.TableName ?? "",
                UserName = p.User != null ? p.User.Name : null,
                MemberName = p.Member != null ? MaskName(p.Member.Name) : "訪客",
                OrderAt = p.OrderAt,
                CompletedAt = p.DoneOrCancel == 1 ? p.Payments.FirstOrDefault(pay => pay.DoneOrCancel == 1)?.PaidAt
                     : p.DoneOrCancel == 2 ? p.CancelledAt
                     : null,
                CompletedAtLabel = p.DoneOrCancel == 1 ? "付款時間"
                     : p.DoneOrCancel == 2 ? "取消時間"
                     : null,
                CouponName = couponName,
                CouponDesc = couponDesc,
                OriginalAmount = p.OriginalAmount,
                DiscountAmount = p.DiscountAmount,
                TotalAmount = p.TotalAmount,
                Note = p.Note,
                PayMethod = p.PayMethod,
                DoneOrCancel = p.DoneOrCancel,
                Items = p.PreOrderDetails.Select(d => new PreOrderDetailItemViewModel
                {
                    DetailId = d.Id,
                    ProductName = d.ProductName,
                    Qty = d.Qty,
                    UnitPrice = d.UnitPrice,
                    Status = d.DoneOrCancel
                }).ToList()
            };
        }

        // ── Payment ──────────────────────────────────────────────────
        public async Task<PaymentCheckoutViewModel> GetCheckoutDetailAsync(int preOrderId)
        {
            var p = await _preOrderRepo.GetByIdAsync(preOrderId);
            if (p == null) return null;

            var servedItems = p.PreOrderDetails.Where(d => d.DoneOrCancel != 2).ToList();
            var originalAmount = servedItems.Sum(d => d.SubTotal);

            return new PaymentCheckoutViewModel
            {
                PreOrderId = p.Id,
                OrderNumber = p.OrderNumber,
                InOrOut = p.InOrOut,
                TableName = p.Table?.TableName ?? "外帶",
                PayMethod = p.PayMethod,
                OriginalAmount = originalAmount,
                CouponName = p.Coupon?.Name,
                DiscountAmount = p.DiscountAmount,
                TotalAmount = originalAmount - p.DiscountAmount,
                HasUnserved = p.PreOrderDetails.Any(d => d.DoneOrCancel == 0),
                Items = p.PreOrderDetails.Select(d => new PaymentDetailItemViewModel
                {
                    DetailId = d.Id,
                    ProductName = d.ProductName,
                    Qty = d.Qty,
                    UnitPrice = d.UnitPrice,
                    SubTotal = d.SubTotal,
                    Status = d.DoneOrCancel,
                    IsSetMeal = d.IsSetMeal,
                    ParentDetailId = d.ParentDetailId
                }).ToList()
            };
        }

        public async Task CancelUnservedDetailsAsync(int preOrderId)
        {
            await _preOrderRepo.CancelUnservedDetailsAsync(preOrderId);
        }

        public async Task<int> CheckoutAsync(int preOrderId, string payMethod)
        {
            var preOrder = await _preOrderRepo.GetByIdAsync(preOrderId);

            var payment = new Payment
            {
                PreOrderId = preOrderId,
                Method = payMethod,       // ← 用傳入的，不用 preOrder.PayMethod
                PaidAt = DateTime.Now,
                DoneOrCancel = 1
            };

            var order = new Order
            {
                PreOrderId = preOrderId,
                OrderNumber = preOrder.OrderNumber,
                MemberId = preOrder.MemberId,
                InOrOut = preOrder.InOrOut,
                TableId = preOrder.TableId,
                UserId = preOrder.UserId,
                OrderAt = preOrder.OrderAt,
                CouponId = preOrder.CouponId,
                OriginalAmount = preOrder.OriginalAmount,
                DiscountAmount = preOrder.DiscountAmount,
                TotalAmount = preOrder.TotalAmount,
                Note = preOrder.Note,
                PayMethod = payMethod,     // ← 用傳入的
                OrderDetails = preOrder.PreOrderDetails
                    .Where(d => d.DoneOrCancel == 1)   // ← 只存已完成
                    .Select(d => new OrderDetail
                    {
                        ProductId = d.ProductId,
                        ProductName = d.ProductName,
                        Qty = d.Qty,
                        UnitPrice = d.UnitPrice,
                        SubTotal = d.SubTotal
                    }).ToList()
            };

            await _orderRepo.AddWithPaymentAsync(order, payment);
            await _preOrderRepo.UpdateStatusAsync(preOrderId, PreOrderStatus.Done);

            foreach (var d in preOrder.PreOrderDetails.Where(d => d.DoneOrCancel == 1))
                await _preOrderRepo.UpdateDetailBilledAsync(d.Id);

            // SplitCheckoutAsync 最後判斷是否關桌
            if (preOrder.TableId.HasValue)
            {
                var freshPending = await _preOrderRepo.GetByStatusAsync(PreOrderStatus.Pending);
                var hasRemaining = freshPending
                    .Where(p => p.TableId == preOrder.TableId && p.DoneOrCancel == 0)
                    .SelectMany(p => p.PreOrderDetails)
                    .Any(d => !d.IsBilled && d.DoneOrCancel != 2);  // ← 改這行

                if (!hasRemaining)
                    await _tableRepo.UpdateStatusAsync(preOrder.TableId.Value, 0);
            }

            return order.Id;
        }

        public async Task<PaymentIndexViewModel> GetPaymentIndexAsync()
        {
            var today = DateTime.Today;
            var tables = await _tableRepo.GetAllAsync();
            var pending = await _preOrderRepo.GetByStatusAsync(PreOrderStatus.Pending);
            var todayDine = pending
                .Where(p => p.InOrOut && p.OrderAt.Date == today)
                .ToList();

            var tableStatuses = tables.Select(t =>
            {
                var orders = todayDine.Where(p => p.TableId == t.Id).ToList();  // ← 取全部
                var firstOrder = orders.FirstOrDefault();

                return new TableStatusViewModel
                {
                    IsOccupied = t.Status == 1,
                    TableId = t.Id,
                    TableName = t.TableName,
                    HasOrder = orders.Any(),                                   // ← 有任一筆就算
                    HasUnserved = orders.SelectMany(o => o.PreOrderDetails)
                                        .Any(d => d.DoneOrCancel == 0),          // ← 合併所有明細
                    PreOrderId = firstOrder?.Id,                                 // 保留第一筆供舊邏輯用
                    TotalAmount = orders.Sum(o => o.TotalAmount)                  // ← 加總所有訂單
                };
            }).ToList();

            var takeoutOrders = pending
                .Where(p => !p.InOrOut && p.OrderAt.Date == today)
                .OrderBy(p => p.OrderAt)
                .Select(p => new PaymentPreOrderSummaryViewModel
                {
                    PreOrderId = p.Id,
                    OrderNumber = p.OrderNumber,
                    InOrOut = p.InOrOut,
                    TableName = "外帶",
                    OrderAt = p.OrderAt,
                    TotalAmount = p.TotalAmount,
                    HasUnserved = p.PreOrderDetails.Any(d => d.DoneOrCancel == 0)
                }).ToList();

            return new PaymentIndexViewModel
            {
                Tables = tableStatuses,
                TakeoutOrders = takeoutOrders
            };
        }

        public async Task<PaymentCheckoutViewModel?> GetCheckoutByTableAsync(int tableId)
        {
            var today = DateTime.Today;
            var orders = await _preOrderRepo.GetByStatusAsync(PreOrderStatus.Pending);
            var tableOrders = orders
                .Where(p => p.InOrOut && p.TableId == tableId && p.OrderAt.Date == today)
                .OrderBy(p => p.OrderAt)
                .ToList();

            if (!tableOrders.Any()) return null;

            // 合併所有訂單的明細
            var allItems = tableOrders
                .SelectMany(p => p.PreOrderDetails.Select(d => new PaymentDetailItemViewModel
                {
                    DetailId = d.Id,
                    ProductName = d.ProductName,
                    Qty = d.Qty,
                    UnitPrice = d.UnitPrice,
                    SubTotal = d.SubTotal,
                    Status = d.DoneOrCancel
                })).ToList();

            var servedItems = tableOrders.SelectMany(p => p.PreOrderDetails.Where(d => d.DoneOrCancel != 2));
            int originalAmount = servedItems.Sum(d => d.SubTotal);
            int discountAmount = tableOrders.Sum(p => p.DiscountAmount);

            // 優惠券顯示第一筆有用券的
            var couponOrder = tableOrders.FirstOrDefault(p => p.Coupon != null);

            return new PaymentCheckoutViewModel
            {
                PreOrderIds = tableOrders.Select(p => p.Id).ToList(),
                PreOrderId = tableOrders.First().Id,
                OrderNumber = tableOrders.Count == 1
                                 ? tableOrders.First().OrderNumber
                                 : $"{tableOrders.First().OrderNumber} 等 {tableOrders.Count} 筆",
                InOrOut = true,
                TableName = tableOrders.First().Table?.TableName ?? "",
                PayMethod = tableOrders.First().PayMethod,
                OriginalAmount = originalAmount,
                DiscountAmount = discountAmount,
                TotalAmount = originalAmount - discountAmount,
                HasUnserved = tableOrders.SelectMany(p => p.PreOrderDetails).Any(d => d.DoneOrCancel == 0),
                CouponName = couponOrder?.Coupon?.Name,
                Items = allItems
            };
        }

        public async Task CancelUnservedByTableAsync(int tableId)
        {
            var today = DateTime.Today;
            var orders = await _preOrderRepo.GetByStatusAsync(PreOrderStatus.Pending);
            var ids = orders
                .Where(p => p.InOrOut && p.TableId == tableId && p.OrderAt.Date == today)
                .Select(p => p.Id).ToList();

            foreach (var id in ids)
                await _preOrderRepo.CancelUnservedDetailsAsync(id);
        }

        public async Task<int> CheckoutByTableAsync(int tableId, string payMethod)
        {
            var today = DateTime.Today;
            var orders = await _preOrderRepo.GetByStatusAsync(PreOrderStatus.Pending);
            var list = orders
                .Where(p => p.InOrOut && p.TableId == tableId && p.OrderAt.Date == today)
                .ToList();

            int lastOrderId = 0;
            foreach (var p in list)
                lastOrderId = await CheckoutAsync(p.Id, payMethod);  // 逐筆結帳

            if (tableId > 0)
                await _tableRepo.UpdateStatusAsync(tableId, 0);

            return lastOrderId;
        }

        public async Task<int> SplitCheckoutAsync(List<int> detailIds, string payMethod)
        {
            // 找到這些 detail 屬於哪些 PreOrder
            var allPending = await _preOrderRepo.GetByStatusAsync(PreOrderStatus.Pending);
            var allDetails = allPending.SelectMany(p => p.PreOrderDetails).ToList();
            var selected = allDetails.Where(d => detailIds.Contains(d.Id)).ToList();

            if (!selected.Any()) return 0;

            // 以第一筆 PreOrder 為主建立 Order
            var firstPreOrderId = selected.First().PreOrderId;
            var preOrder = allPending.First(p => p.Id == firstPreOrderId);

            int originalAmount = selected.Sum(d => d.SubTotal);

            var payment = new Payment
            {
                PreOrderId = firstPreOrderId,
                Method = payMethod,
                PaidAt = DateTime.Now,
                DoneOrCancel = 1
            };

            var order = new Order
            {
                PreOrderId = firstPreOrderId,
                OrderNumber = preOrder.OrderNumber + "-S" + DateTime.Now.ToString("mmss"),
                MemberId = preOrder.MemberId,
                InOrOut = preOrder.InOrOut,
                TableId = preOrder.TableId,
                UserId = preOrder.UserId,
                OrderAt = preOrder.OrderAt,
                OriginalAmount = originalAmount,
                DiscountAmount = 0,   // 拆單不套用優惠券
                TotalAmount = originalAmount,
                Note = preOrder.Note,
                PayMethod = payMethod,
                OrderDetails = selected.Select(d => new OrderDetail
                {
                    ProductId = d.ProductId,
                    ProductName = d.ProductName,
                    Qty = d.Qty,
                    UnitPrice = d.UnitPrice,
                    SubTotal = d.SubTotal
                }).ToList()
            };

            await _orderRepo.AddWithPaymentAsync(order, payment);

            // 把已結帳的 detail 標記為完成
            foreach (var d in selected)
                await _preOrderRepo.UpdateDetailStatusAsync(d.Id, 1);

            // 檢查每筆 PreOrder 是否全部結完
            foreach (var preOrderId in selected.Select(d => d.PreOrderId).Distinct())
            {
                var p = await _preOrderRepo.GetByIdAsync(preOrderId);
                if (p != null && p.PreOrderDetails.All(d => d.DoneOrCancel == 1))
                {
                    await _preOrderRepo.UpdateStatusAsync(preOrderId, PreOrderStatus.Done);
                }
            }

            // 如果該桌所有 PreOrder 都結完，桌子改回空桌
            var tableId = preOrder.TableId;
            if (tableId.HasValue)
            {
                var remaining = allPending
                    .Where(p => p.TableId == tableId && p.DoneOrCancel == 0)
                    .SelectMany(p => p.PreOrderDetails)
                    .Any(d => d.DoneOrCancel == 0);

                if (!remaining)
                    await _tableRepo.UpdateStatusAsync(tableId.Value, 0);
            }

            return order.Id;
        }

        public async Task<PaymentCheckoutViewModel?> GetCheckoutByTableAsync(int tableId)
        {
            var today = DateTime.Today;
            var orders = await _preOrderRepo.GetByStatusAsync(PreOrderStatus.Pending);
            var tableOrders = orders
                .Where(p => p.InOrOut && p.TableId == tableId && p.OrderAt.Date == today)
                .OrderBy(p => p.OrderAt)
                .ToList();

            if (!tableOrders.Any()) return null;

            // 只顯示還沒結帳的餐點（IsBilled = false 且沒有被取消）
            var allItems = tableOrders
                .SelectMany(p => p.PreOrderDetails
                .Select(d => new PaymentDetailItemViewModel
                {
                    DetailId = d.Id,
                    ProductName = d.ProductName,
                    Qty = d.Qty,
                    UnitPrice = d.UnitPrice,
                    SubTotal = d.SubTotal,
                    Status = d.DoneOrCancel,
                    IsBilled = d.IsBilled,
                    IsSetMeal = d.IsSetMeal,
                    ParentDetailId = d.ParentDetailId
                }))
                .ToList();

            // 沒有任何未結餐點就回 null
            if (!allItems.Any()) return null;

            // 判斷是否還有可結帳的餐點
            var billableItems = allItems.Where(d => !d.IsBilled && d.Status != 2).ToList();
            if (!billableItems.Any()) return null;  // 全部結完或取消才回 null

            var servedItems = tableOrders.SelectMany(p => p.PreOrderDetails.Where(d => d.DoneOrCancel != 2));
            int originalAmount = billableItems.Sum(d => d.SubTotal);
            int discountAmount = tableOrders.Sum(p => p.DiscountAmount);

            // 優惠券顯示第一筆有用券的
            var couponOrder = tableOrders.FirstOrDefault(p => p.Coupon != null);

            return new PaymentCheckoutViewModel
            {
                PreOrderIds = tableOrders.Select(p => p.Id).ToList(),
                PreOrderId = tableOrders.First().Id,
                OrderNumber = tableOrders.Count == 1
                                 ? tableOrders.First().OrderNumber
                                 : $"{tableOrders.First().OrderNumber} 等 {tableOrders.Count} 筆",
                InOrOut = true,
                TableName = tableOrders.First().Table?.TableName ?? "",
                PayMethod = tableOrders.First().PayMethod,
                OriginalAmount = originalAmount,
                DiscountAmount = discountAmount,
                TotalAmount = originalAmount - discountAmount,
                HasUnserved = allItems.Any(d => d.Status == 0 && !d.IsBilled),
                CouponName = couponOrder?.Coupon?.Name,
                Items = allItems
            };
        }

        public async Task CancelUnservedByTableAsync(int tableId)
        {
            var today = DateTime.Today;
            var orders = await _preOrderRepo.GetByStatusAsync(PreOrderStatus.Pending);
            var ids = orders
                .Where(p => p.InOrOut && p.TableId == tableId && p.OrderAt.Date == today)
                .Select(p => p.Id).ToList();

            foreach (var id in ids)
                await _preOrderRepo.CancelUnservedDetailsAsync(id);
        }

        public async Task<int> CheckoutByTableAsync(int tableId, string payMethod)
        {
            var today = DateTime.Today;
            var orders = await _preOrderRepo.GetByStatusAsync(PreOrderStatus.Pending);
            var list = orders
                .Where(p => p.InOrOut && p.TableId == tableId && p.OrderAt.Date == today)
                .ToList();

            int lastOrderId = 0;
            foreach (var p in list)
                lastOrderId = await CheckoutAsync(p.Id, payMethod);  // 逐筆結帳

            return lastOrderId;
        }

        public async Task<int> SplitCheckoutAsync(List<int> detailIds, string payMethod)
        {
            // 找到這些 detail 屬於哪些 PreOrder
            var allPending = await _preOrderRepo.GetByStatusAsync(PreOrderStatus.Pending);
            var allDetails = allPending.SelectMany(p => p.PreOrderDetails).ToList();
            var selected = allDetails.Where(d => detailIds.Contains(d.Id)).ToList();

            if (!selected.Any()) return 0;

            // 以第一筆 PreOrder 為主建立 Order
            var firstPreOrderId = selected.First().PreOrderId;
            var preOrder = allPending.First(p => p.Id == firstPreOrderId);

            int originalAmount = selected.Sum(d => d.SubTotal);

            var payment = new Payment
            {
                PreOrderId = firstPreOrderId,
                Method = payMethod,
                PaidAt = DateTime.Now,
                DoneOrCancel = 1
            };

            var order = new Order
            {
                PreOrderId = firstPreOrderId,
                OrderNumber = preOrder.OrderNumber + "-S" + DateTime.Now.ToString("mmss"),
                MemberId = preOrder.MemberId,
                InOrOut = preOrder.InOrOut,
                TableId = preOrder.TableId,
                UserId = preOrder.UserId,
                OrderAt = preOrder.OrderAt,
                OriginalAmount = originalAmount,
                DiscountAmount = 0,   // 拆單不套用優惠券
                TotalAmount = originalAmount,
                Note = preOrder.Note,
                PayMethod = payMethod,
                OrderDetails = selected.Select(d => new OrderDetail
                {
                    ProductId = d.ProductId,
                    ProductName = d.ProductName,
                    Qty = d.Qty,
                    UnitPrice = d.UnitPrice,
                    SubTotal = d.SubTotal
                }).ToList()
            };

            await _orderRepo.AddWithPaymentAsync(order, payment);

            foreach (var d in selected)
                await _preOrderRepo.UpdateDetailBilledAsync(d.Id);

            // 檢查每筆 PreOrder 是否所有非取消餐點都已結帳
            foreach (var preOrderId in selected.Select(d => d.PreOrderId).Distinct())
            {
                var allBilled = await _preOrderRepo.AllNonCancelledDetailsBilledAsync(preOrderId);
                if (allBilled)
                    await _preOrderRepo.UpdateStatusAsync(preOrderId, PreOrderStatus.Done);
            }

            // 如果該桌所有 PreOrder 都結完，桌子改回空桌
            if (preOrder.TableId.HasValue)
            {
                var hasRemaining = await _preOrderRepo
                    .HasUnbilledDetailsForTableAsync(preOrder.TableId.Value);

                if (!hasRemaining)
                    await _tableRepo.UpdateStatusAsync(preOrder.TableId.Value, 0);
            }

            return order.Id;
        }

        public async Task UpdateOrderTableAsync(int preOrderId, int? newTableId, bool inOrOut)
        {
            var order = await _preOrderRepo.GetByIdAsync(preOrderId);
            if (order == null) return;

            // 原本是內用 → 把舊桌位改回空桌
            if (order.TableId.HasValue)
                await _tableRepo.UpdateStatusAsync(order.TableId.Value, 0);

            // 新桌位是內用 → 把新桌位改為用餐中
            if (newTableId.HasValue)
                await _tableRepo.UpdateStatusAsync(newTableId.Value, 1);

            await _preOrderRepo.UpdateTableAsync(preOrderId, newTableId, inOrOut);
        }
    }
}