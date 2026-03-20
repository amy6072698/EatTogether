using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.Repositories;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

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
        Task<List<EventApplicableDto>> GetApplicableEventsAsync(int amount);
        Task<bool> HasActiveOrderForTableAsync(int tableId);

        // Checkout discount selection
        Task<List<EventApplicableDto>> GetManualEventsForOrderAsync(int? tableId, int? preOrderId);
        Task<List<CouponDto>> GetApplicableCouponsForOrderAsync(int? tableId, int? preOrderId);
        Task<(bool Success, string? Error, PaymentCheckoutViewModel? Data)> ApplyEventToOrderAsync(int? tableId, int? preOrderId, int? eventId);
        Task<(bool Success, string? Error, PaymentCheckoutViewModel? Data)> ApplyCouponToOrderAsync(int? tableId, int? preOrderId, string couponCode);
        Task<PaymentCheckoutViewModel?> ApplyCouponByIdToOrderAsync(int? tableId, int? preOrderId, int? couponId);
    }
    public class OrderService : IOrderService
    {
        private readonly IPreOrderRepository _preOrderRepo;
        private readonly ITableRepository _tableRepo;
        private readonly IProductRepository _productRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly ICouponRepository _couponRepo;
        private readonly IEventRepository _eventRepo;

        public OrderService(
            IPreOrderRepository preOrderRepo,
            ITableRepository tableRepo,
            IProductRepository productRepo,
            IOrderRepository orderRepo,
            ICouponRepository couponRepo,
            IEventRepository eventRepo)
        {
            _preOrderRepo = preOrderRepo;
            _tableRepo = tableRepo;
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _couponRepo = couponRepo;
            _eventRepo = eventRepo;
        }

        // ── CreatePreOrder ──────────────────────────────────────────────────
        public async Task<string> CreatePreOrderAsync(CreatePreOrderDto dto)
        {
            var orderNumber = await GenerateOrderNumberAsync();

            // 先算原始金額（展開前）
            var originalAmount = dto.Items
                .Where(i => !i.ParentIndex.HasValue)
                .Sum(i => i.Qty * i.UnitPrice);
            var discountAmount = dto.DiscountAmount;

            // 加點不重複套用贈品活動
            var allGiftEvents = dto.IsAddOrder
                ? new List<EventApplicableDto>()
                : await _eventRepo.GetApplicableEventsAsync((int)originalAmount);
            foreach (var giftEv in allGiftEvents.Where(e => e.DiscountType == "Gift" && !string.IsNullOrEmpty(e.RewardDishName)))
            {
                dto.Items.Add(new PreOrderDetailDto
                {
                    ProductId   = 0,
                    ProductName = $"🎁 {giftEv.RewardDishName}（活動贈品）",
                    Qty         = 1,
                    UnitPrice   = 0,
                    IsSetMeal   = false,
                    ParentIndex = null
                });
            }

            // ── 展開：每份拆成獨立一筆（Qty=1），方便廚房逐份追蹤 ──
            // 記錄舊 index -> 展開後的 new index 清單
            var parentNewIndices = new Dictionary<int, List<int>>();
            var expandedItems = new List<PreOrderDetailDto>();

            for (int i = 0; i < dto.Items.Count; i++)
            {
                var item = dto.Items[i];
                if (item.ParentIndex.HasValue) continue; // 子項目稍後處理

                parentNewIndices[i] = new List<int>();
                int repeat = Math.Max(1, item.Qty);
                for (int q = 0; q < repeat; q++)
                {
                    parentNewIndices[i].Add(expandedItems.Count);
                    expandedItems.Add(new PreOrderDetailDto
                    {
                        ProductId   = item.ProductId,
                        ProductName = item.ProductName,
                        Qty         = 1,
                        UnitPrice   = item.UnitPrice,
                        IsSetMeal   = item.IsSetMeal,
                        ParentIndex = null
                    });
                }
            }

            // 子項目：每個父項目實例各複製一份子項目
            for (int i = 0; i < dto.Items.Count; i++)
            {
                var item = dto.Items[i];
                if (!item.ParentIndex.HasValue) continue;
                if (!parentNewIndices.ContainsKey(item.ParentIndex.Value)) continue;

                foreach (var newParentIdx in parentNewIndices[item.ParentIndex.Value])
                {
                    expandedItems.Add(new PreOrderDetailDto
                    {
                        ProductId   = item.ProductId,
                        ProductName = item.ProductName,
                        Qty         = 1,
                        UnitPrice   = item.UnitPrice,
                        IsSetMeal   = false,
                        ParentIndex = newParentIdx
                    });
                }
            }

            // 第一階段：建立 detail 列表
            var details = expandedItems.Select(i => new PreOrderDetail
            {
                ProductId    = i.ProductId > 0 ? i.ProductId : 1,
                ProductName  = i.ProductName,
                Qty          = 1,
                UnitPrice    = (int)i.UnitPrice,
                SubTotal     = i.ParentIndex.HasValue ? 0 : (int)i.UnitPrice,
                IsSetMeal    = i.IsSetMeal,
                DoneOrCancel = 0
            }).ToList();

            var preOrder = new PreOrder
            {
                OrderNumber    = orderNumber,
                InOrOut        = dto.InOrOut,
                TableId        = dto.InOrOut ? dto.TableId : null,
                OrderAt        = DateTime.Now,
                OriginalAmount = (int)originalAmount,
                CouponId       = dto.CouponId,
                EventId        = dto.EventId,
                DiscountAmount = discountAmount,
                TotalAmount    = (int)(originalAmount - discountAmount),
                Note           = dto.Note,
                PeopleNum      = dto.PeopleNum,
                PayMethod      = dto.PayMethod,
                DoneOrCancel   = PreOrderStatus.Pending,
                PreOrderDetails = details
            };

            await _preOrderRepo.AddAsync(preOrder);

            // 第二階段：設定子項目的 ParentDetailId
            bool hasChildren = false;
            for (int i = 0; i < expandedItems.Count; i++)
            {
                if (expandedItems[i].ParentIndex.HasValue)
                {
                    details[i].ParentDetailId = details[expandedItems[i].ParentIndex.Value].Id;
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
                // 支付方式中文關鍵字對應
                var payMethods = new List<string>();
                if ("現金".Contains(kw) || kw.Contains("現金")) payMethods.Add("Cash");
                if ("刷卡".Contains(kw) || kw.Contains("刷卡")) payMethods.Add("Card");
                if ("行動支付".Contains(kw) || kw.Contains("行動") || kw.Contains("支付")) payMethods.Add("LinePay");

                filtered = filtered.Where(p =>
                    p.OrderNumber.Contains(kw) ||
                    (p.Member != null && p.Member.Name.Contains(kw)) ||
                    (p.Table != null && p.Table.TableName.Contains(kw)) ||
                    (p.PayMethod != null && (p.PayMethod.Contains(kw) || payMethods.Contains(p.PayMethod))) ||
                    (p.Coupon != null && p.Coupon.Name.Contains(kw)) ||
                    (p.Event != null && p.Event.Title.Contains(kw))
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
                                 : null,
                    EventTitle = p.Event != null ? p.Event.Title : null,
                    PeopleNum = p.PeopleNum
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
            var preOrder = await _preOrderRepo.GetByIdAsync(preOrderId);
            await _preOrderRepo.CancelEntireOrderAsync(preOrderId);

            // 同桌沒有其他 Pending 訂單才改回空桌
            if (preOrder?.TableId.HasValue == true)
            {
                var hasOther = await _preOrderRepo.HasUnbilledDetailsForTableAsync(preOrder.TableId.Value);
                if (!hasOther)
                    await _tableRepo.UpdateStatusAsync(preOrder.TableId.Value, 0);
            }
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
                EventTitle = p.Event?.Title,
                PeopleNum = p.PeopleNum,
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

            // 未取消的餐點小計（禮品 SubTotal=0 不計入）
            var originalAmount = p.PreOrderDetails
                .Where(d => d.DoneOrCancel != 2 && d.SubTotal > 0)
                .Sum(d => d.SubTotal);

            // ── 贈品活動：門檻不足 → 贈品以原價計入（需補差價）────────────────
            bool giftEventInvalid = false;
            if (p.EventId.HasValue)
            {
                var ev = await _eventRepo.GetEditByIdAsync(p.EventId.Value);
                if (ev != null && ev.DiscountType == "Gift" && ev.MinSpend > originalAmount)
                    giftEventInvalid = true;
            }

            var invalidGiftDetails = new Dictionary<int, int>(); // detailId → 補差價售價
            if (giftEventInvalid)
            {
                foreach (var d in p.PreOrderDetails.Where(d =>
                    d.UnitPrice == 0 && d.ProductName.Contains("活動贈品") && d.DoneOrCancel != 2))
                {
                    var rawName = d.ProductName.Replace("🎁 ", "").Replace("（活動贈品）", "").Trim();
                    var price = await _productRepo.GetPriceByNameAsync(rawName);
                    invalidGiftDetails[d.Id] = price ?? 0;
                }
                // 贈品以原價加回總計（視為客人主動放棄免費資格）
                originalAmount += invalidGiftDetails.Values.Sum();
            }

            // ── 重新驗證非贈品活動＆優惠券 MinSpend ─────────────────────────────
            var discountResult = await ComputeDiscountAsync(new List<PreOrder> { p }, originalAmount);
            int discountAmount = discountResult.Amount;

            return new PaymentCheckoutViewModel
            {
                PreOrderId = p.Id,
                OrderNumber = p.OrderNumber,
                InOrOut = p.InOrOut,
                TableName = p.Table?.TableName ?? "外帶",
                PayMethod = p.PayMethod,
                OriginalAmount = originalAmount,
                CouponId   = discountResult.InvalidCouponOrderIds.Contains(p.Id) ? null : p.CouponId,
                CouponName = discountResult.InvalidCouponOrderIds.Contains(p.Id) ? null : p.Coupon?.Name,
                EventId    = (discountResult.InvalidEventOrderIds.Contains(p.Id) || giftEventInvalid) ? null : p.EventId,
                EventTitle = (discountResult.InvalidEventOrderIds.Contains(p.Id) || giftEventInvalid)
                             ? null : p.Event?.Title,
                DiscountAmount = discountAmount,
                TotalAmount = originalAmount - discountAmount,
                HasUnserved = p.PreOrderDetails.Any(d => d.DoneOrCancel == 0),
                Items = p.PreOrderDetails.Select(d => new PaymentDetailItemViewModel
                {
                    DetailId  = d.Id,
                    ProductName = d.ProductName,
                    Qty       = d.Qty,
                    UnitPrice = invalidGiftDetails.TryGetValue(d.Id, out var giftPrice) ? giftPrice : d.UnitPrice,
                    SubTotal  = invalidGiftDetails.TryGetValue(d.Id, out var giftSub)   ? giftSub   : d.SubTotal,
                    Status    = d.DoneOrCancel,
                    IsSetMeal = d.IsSetMeal,
                    ParentDetailId = d.ParentDetailId,
                    IsInvalidGift  = invalidGiftDetails.ContainsKey(d.Id)
                }).ToList()
            };
        }

        /// <summary>
        /// 取消未出餐前的前置處理：
        /// 1. 套餐部分出餐 → 已出餐子項改為單點計費並脫離套餐
        /// 2. Gift 型活動 → 若改單點後仍符合門檻，保留贈品 item（不取消）；否則清除活動
        /// 回傳「應保留（不取消）的 DetailId 集合」
        /// </summary>
        private async Task<ISet<int>> PrepareUnservedCancellationAsync(int preOrderId)
        {
            var preOrder = await _preOrderRepo.GetByIdAsync(preOrderId);
            if (preOrder == null) return new HashSet<int>();

            var allDetails = preOrder.PreOrderDetails.ToList();

            // ── Step 1：套餐子項改單點計費 ──────────────────────────────────
            var cancelledSetMealParentIds = allDetails
                .Where(d => d.IsSetMeal && d.DoneOrCancel == 0)
                .Select(d => d.Id)
                .ToHashSet();

            if (cancelledSetMealParentIds.Any())
            {
                var servedChildren = allDetails
                    .Where(d => d.ParentDetailId.HasValue
                             && cancelledSetMealParentIds.Contains(d.ParentDetailId.Value)
                             && d.DoneOrCancel == 1)
                    .ToList();

                foreach (var child in servedChildren)
                {
                    var price        = await _productRepo.GetPriceByNameAsync(child.ProductName) ?? 0;
                    child.UnitPrice  = price;
                    child.SubTotal   = price;
                    child.ParentDetailId = null;   // 脫離套餐，成獨立計費項目
                }
            }

            // ── Step 2：Gift 活動贈品保留判斷 ───────────────────────────────
            var giftIdsToPreserve = new HashSet<int>();

            if (preOrder.EventId.HasValue && preOrder.Event?.DiscountType == "Gift")
            {
                // 取消後的剩餘應付金額（已出餐、SubTotal > 0、未結帳，含剛改好的單點子項）
                var remainingAmount = allDetails
                    .Where(d => d.DoneOrCancel == 1 && d.SubTotal > 0 && !d.IsBilled)
                    .Sum(d => d.SubTotal);

                var giftItems = allDetails
                    .Where(d => d.DoneOrCancel == 0
                             && d.UnitPrice == 0 && d.SubTotal == 0
                             && d.ProductName.Contains("活動贈品"))
                    .ToList();

                if (preOrder.Event.MinSpend <= remainingAmount)
                {
                    // 仍符合門檻 → 贈品保留（不取消）
                    foreach (var g in giftItems)
                        giftIdsToPreserve.Add(g.Id);
                }
                else
                {
                    // 不再符合門檻 → 清除活動綁定
                    preOrder.EventId = null;
                }
            }

            // 注意：此處不 SaveChanges；由呼叫端的 CancelUnservedDetailsAsync 一起儲存
            return giftIdsToPreserve;
        }

        public async Task CancelUnservedDetailsAsync(int preOrderId)
        {
            var giftIdsToPreserve = await PrepareUnservedCancellationAsync(preOrderId);
            await _preOrderRepo.CancelUnservedDetailsAsync(preOrderId, giftIdsToPreserve);
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

            // 改為 async foreach，以便對每桌呼叫 ComputeDiscountAsync
            var tableStatuses = new List<TableStatusViewModel>();
            foreach (var t in tables)
            {
                var orders = todayDine.Where(p => p.TableId == t.Id).ToList();
                var firstOrder = orders.FirstOrDefault();

                var unbilledAmount = orders
                    .SelectMany(o => o.PreOrderDetails)
                    .Where(d => !d.IsBilled && d.DoneOrCancel != 2 && d.SubTotal > 0)
                    .Sum(d => d.SubTotal);

                var discount = orders.Any()
                    ? (await ComputeDiscountAsync(orders, unbilledAmount)).Amount
                    : 0;

                tableStatuses.Add(new TableStatusViewModel
                {
                    IsOccupied = t.Status == 1,
                    TableId = t.Id,
                    TableName = t.TableName,
                    HasOrder = orders.Any(),
                    HasUnserved = orders.SelectMany(o => o.PreOrderDetails)
                                        .Any(d => d.DoneOrCancel == 0),
                    PreOrderId = firstOrder?.Id,
                    TotalAmount = Math.Max(0, unbilledAmount - discount)
                });
            }

            var takeoutOrders = new List<PaymentPreOrderSummaryViewModel>();
            foreach (var p in pending.Where(o => !o.InOrOut && o.OrderAt.Date == today).OrderBy(o => o.OrderAt))
            {
                var unbilled = p.PreOrderDetails
                    .Where(d => !d.IsBilled && d.DoneOrCancel != 2 && d.SubTotal > 0)
                    .Sum(d => d.SubTotal);
                var discount = (await ComputeDiscountAsync(new List<PreOrder> { p }, unbilled)).Amount;

                takeoutOrders.Add(new PaymentPreOrderSummaryViewModel
                {
                    PreOrderId = p.Id,
                    OrderNumber = p.OrderNumber,
                    InOrOut = p.InOrOut,
                    TableName = "外帶",
                    OrderAt = p.OrderAt,
                    TotalAmount = Math.Max(0, unbilled - discount),
                    HasUnserved = p.PreOrderDetails.Any(d => d.DoneOrCancel == 0)
                });
            }

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

            // 只顯示還沒結帳的餐點（IsBilled = false 且沒有被取消）
            var allItems = tableOrders
                .SelectMany(p => p.PreOrderDetails
                .Select(d => new PaymentDetailItemViewModel
                {
                    PreOrderId = p.Id,
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

            // 判斷是否還有可結帳的餐點（NT$0 贈品不計入金額門檻）
            var billableItems = allItems.Where(d => !d.IsBilled && d.Status != 2 && d.SubTotal > 0).ToList();
            if (!billableItems.Any()) return null;  // 全部結完或取消才回 null

            int originalAmount = billableItems.Sum(d => d.SubTotal);

            // 重新依目前未結金額判斷活動＆優惠券是否仍符合門檻
            var discountResult = await ComputeDiscountAsync(tableOrders, originalAmount);
            int discountAmount = discountResult.Amount;

            // 優惠券/活動標籤：只顯示仍有效的那筆
            var couponOrder = tableOrders.FirstOrDefault(p =>
                p.Coupon != null && !discountResult.InvalidCouponOrderIds.Contains(p.Id));
            var eventOrder  = tableOrders.FirstOrDefault(p =>
                p.Event != null &&
                !discountResult.InvalidEventOrderIds.Contains(p.Id));

            var subOrders = tableOrders.Select(p => new SubOrderSummary
            {
                PreOrderId  = p.Id,
                OrderNumber = p.OrderNumber,
                Amount      = p.PreOrderDetails
                               .Where(d => !d.IsBilled && d.DoneOrCancel != 2 && d.SubTotal > 0)
                               .Sum(d => d.SubTotal),
                HasUnserved = p.PreOrderDetails.Any(d => d.DoneOrCancel == 0 && !d.IsBilled)
            }).ToList();

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
                CouponId   = couponOrder?.CouponId,
                CouponName = couponOrder?.Coupon?.Name,
                EventId    = eventOrder?.EventId,
                EventTitle = eventOrder?.Event?.Title,
                Items      = allItems,
                SubOrders  = subOrders
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
                await CancelUnservedDetailsAsync(id);   // 走 service 層，含單點改價與活動贈品保留
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

            // 自動將 NT$0 贈品一起標為已結帳（贈品不需獨立拆單）
            var giftItems = allDetails
                .Where(d => d.UnitPrice == 0 && d.SubTotal == 0 && !d.IsBilled && d.DoneOrCancel != 2)
                .ToList();
            foreach (var d in giftItems)
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

        public async Task<List<EventApplicableDto>> GetApplicableEventsAsync(int amount)
            => await _eventRepo.GetApplicableEventsAsync(amount);

        // ── 結帳頁：手動選擇活動／優惠券 ───────────────────────────────────────
        private async Task<List<PreOrder>> GetOrdersForContextAsync(int? tableId, int? preOrderId)
        {
            if (tableId.HasValue)
                return await _preOrderRepo.GetActiveByTableIdAsync(tableId.Value);
            if (preOrderId.HasValue)
            {
                var order = await _preOrderRepo.GetByIdAsync(preOrderId.Value);
                return order != null ? new List<PreOrder> { order } : new List<PreOrder>();
            }
            return new List<PreOrder>();
        }

        public async Task<List<EventApplicableDto>> GetManualEventsForOrderAsync(int? tableId, int? preOrderId)
        {
            var orders = await GetOrdersForContextAsync(tableId, preOrderId);
            int unbilledAmount = orders
                .SelectMany(o => o.PreOrderDetails)
                .Where(d => !d.IsBilled && d.DoneOrCancel != 2 && d.SubTotal > 0)
                .Sum(d => d.SubTotal);

            var eligible = await _eventRepo.GetManualEventsAsync(unbilledAmount);

            // 目前使用中的活動 ID
            var inUseIds = orders
                .Where(o => o.EventId.HasValue)
                .Select(o => o.EventId!.Value)
                .ToHashSet();

            // 標記符合門檻清單中已使用的
            foreach (var ev in eligible)
                if (inUseIds.Contains(ev.Id))
                    ev.IsInUse = true;

            // 使用中但不在符合門檻清單的（例如後來金額降低），補入並標記
            var missingIds = inUseIds.Except(eligible.Select(e => e.Id)).ToList();
            if (missingIds.Count > 0)
            {
                var extra = await _eventRepo.GetEventsByIdsAsync(missingIds);
                foreach (var ev in extra) ev.IsInUse = true;
                eligible.AddRange(extra);
            }

            return eligible;
        }

        public async Task<List<CouponDto>> GetApplicableCouponsForOrderAsync(int? tableId, int? preOrderId)
        {
            var orders = await GetOrdersForContextAsync(tableId, preOrderId);
            int unbilledAmount = orders
                .SelectMany(o => o.PreOrderDetails)
                .Where(d => !d.IsBilled && d.DoneOrCancel != 2 && d.SubTotal > 0)
                .Sum(d => d.SubTotal);

            var eligible = await _couponRepo.GetApplicableCouponsAsync(unbilledAmount);

            // 目前使用中的優惠券 ID
            var inUseIds = orders
                .Where(o => o.CouponId.HasValue)
                .Select(o => o.CouponId!.Value)
                .ToHashSet();

            foreach (var c in eligible)
                if (inUseIds.Contains(c.Id))
                    c.IsInUse = true;

            var missingIds = inUseIds.Except(eligible.Select(c => c.Id)).ToList();
            if (missingIds.Count > 0)
            {
                var extra = await _couponRepo.GetCouponsByIdsAsync(missingIds);
                foreach (var c in extra) c.IsInUse = true;
                eligible.AddRange(extra);
            }

            return eligible;
        }

        public async Task<PaymentCheckoutViewModel?> ApplyCouponByIdToOrderAsync(int? tableId, int? preOrderId, int? couponId)
        {
            var orders = await GetOrdersForContextAsync(tableId, preOrderId);
            if (!orders.Any()) return null;

            if (!couponId.HasValue)
            {
                // ── 清除模式：移除所有優惠券 ──────────────────────────────────
                foreach (var o in orders)
                {
                    o.CouponId       = null;
                    o.DiscountAmount = 0;
                }
            }
            else
            {
                // ── Toggle 模式：已套用則移除，未套用則新增 ──────────────────
                var existingOrder = orders.FirstOrDefault(o => o.CouponId == couponId);
                if (existingOrder != null)
                {
                    // 已套用 → 移除（toggle off）
                    existingOrder.CouponId       = null;
                    existingOrder.DiscountAmount = 0;
                }
                else
                {
                    // 未套用 → 新增到第一筆沒有優惠券的訂單，否則覆蓋第一筆
                    var target = orders.FirstOrDefault(o => !o.CouponId.HasValue) ?? orders.First();
                    target.CouponId = couponId;
                }
            }

            await _preOrderRepo.SaveChangesAsync();

            if (tableId.HasValue)    return await GetCheckoutByTableAsync(tableId.Value);
            if (preOrderId.HasValue) return await GetCheckoutDetailAsync(preOrderId.Value);
            return null;
        }

        private static void CancelPendingGifts(PreOrder order)
        {
            foreach (var g in order.PreOrderDetails
                .Where(d => d.UnitPrice == 0 && d.SubTotal == 0
                         && d.DoneOrCancel == 0
                         && d.ProductName.Contains("活動贈品")).ToList())
                g.DoneOrCancel = 2;
        }

        /// <summary>
        /// 檢查某筆訂單「目前活動」的贈品是否已出餐（精確比對品名，避免舊活動遺留項目誤判）。
        /// </summary>
        private async Task<bool> IsCurrentEventGiftServedAsync(PreOrder order)
        {
            if (!order.EventId.HasValue) return false;
            var giftInfo = await _eventRepo.GetEventGiftInfoAsync(order.EventId.Value);
            if (giftInfo?.DiscountType != "Gift" || string.IsNullOrEmpty(giftInfo?.RewardDishName))
                return false;

            var expectedName = $"🎁 {giftInfo.Value.RewardDishName}（活動贈品）";
            return order.PreOrderDetails.Any(d =>
                d.DoneOrCancel == 1 && d.ProductName == expectedName);
        }

        public async Task<(bool Success, string? Error, PaymentCheckoutViewModel? Data)> ApplyEventToOrderAsync(int? tableId, int? preOrderId, int? eventId)
        {
            var orders = await GetOrdersForContextAsync(tableId, preOrderId);
            if (!orders.Any()) return (false, "找不到訂單", null);

            if (!eventId.HasValue)
            {
                // ── 清除模式：移除所有活動與對應待出贈品 ─────────────────────
                foreach (var o in orders)
                {
                    if (await IsCurrentEventGiftServedAsync(o))
                        return (false, "贈品已出餐，無法清除活動", null);
                    CancelPendingGifts(o);
                    o.EventId        = null;
                    o.DiscountAmount = 0;
                }
            }
            else
            {
                // ── Toggle 模式：已套用則移除，未套用則新增/覆蓋 ─────────────
                var existingOrder = orders.FirstOrDefault(o => o.EventId == eventId);
                if (existingOrder != null)
                {
                    // 已套用 → 移除（toggle off）
                    if (await IsCurrentEventGiftServedAsync(existingOrder))
                        return (false, "贈品已出餐，無法取消活動", null);
                    CancelPendingGifts(existingOrder);
                    existingOrder.EventId        = null;
                    existingOrder.DiscountAmount = 0;
                }
                else
                {
                    // 未套用 → 找空位；沒有空位則覆蓋第一筆
                    var target = orders.FirstOrDefault(o => !o.EventId.HasValue) ?? orders.First();

                    // 若覆蓋同一筆中的舊活動，先確認舊活動的贈品沒有已出餐
                    if (target.EventId.HasValue)
                    {
                        if (await IsCurrentEventGiftServedAsync(target))
                            return (false, "贈品已出餐，無法更換活動", null);
                        CancelPendingGifts(target);
                    }

                    target.EventId = eventId;

                    // 若為 Gift 型，加贈品明細
                    var giftInfo = await _eventRepo.GetEventGiftInfoAsync(eventId.Value);
                    if (giftInfo?.DiscountType == "Gift" && !string.IsNullOrEmpty(giftInfo?.RewardDishName))
                    {
                        target.PreOrderDetails.Add(new EfModels.PreOrderDetail
                        {
                            PreOrderId   = target.Id,
                            ProductId    = 1,
                            ProductName  = $"🎁 {giftInfo.Value.RewardDishName}（活動贈品）",
                            UnitPrice    = 0,
                            Qty          = 1,
                            SubTotal     = 0,
                            DoneOrCancel = 0,
                            IsSetMeal    = false
                        });
                    }
                }
            }

            await _preOrderRepo.SaveChangesAsync();

            PaymentCheckoutViewModel? vm = tableId.HasValue
                ? await GetCheckoutByTableAsync(tableId.Value)
                : preOrderId.HasValue ? await GetCheckoutDetailAsync(preOrderId.Value) : null;

            return (true, null, vm);
        }

        public async Task<(bool Success, string? Error, PaymentCheckoutViewModel? Data)> ApplyCouponToOrderAsync(int? tableId, int? preOrderId, string couponCode)
        {
            var orders = await GetOrdersForContextAsync(tableId, preOrderId);
            if (!orders.Any()) return (false, "找不到訂單", null);

            int unbilledAmount = orders
                .SelectMany(o => o.PreOrderDetails)
                .Where(d => !d.IsBilled && d.DoneOrCancel != 2 && d.SubTotal > 0)
                .Sum(d => d.SubTotal);

            var coupon = await _couponRepo.GetByCodeAsync(couponCode?.Trim() ?? "");
            if (coupon == null)
                return (false, "折扣碼無效或已過期", null);
            if (coupon.IsDisabled)
                return (false, "此折扣碼已停用", null);
            if (unbilledAmount < coupon.MinSpend)
                return (false, $"未達最低消費 NT$ {coupon.MinSpend}（目前 NT$ {unbilledAmount}）", null);

            // 保留現有活動
            int? existingEventId = orders.FirstOrDefault(o => o.EventId.HasValue)?.EventId;

            // 清除所有訂單的活動/優惠券/折扣
            foreach (var o in orders)
            {
                o.EventId        = null;
                o.CouponId       = null;
                o.DiscountAmount = 0;
            }

            // 套用優惠券（與保留的活動）到第一筆
            var first = orders.First();
            first.CouponId = coupon.Id;
            if (existingEventId.HasValue)
                first.EventId = existingEventId;

            await _preOrderRepo.SaveChangesAsync();

            PaymentCheckoutViewModel? vm = null;
            if (tableId.HasValue)    vm = await GetCheckoutByTableAsync(tableId.Value);
            else if (preOrderId.HasValue) vm = await GetCheckoutDetailAsync(preOrderId.Value);

            return (true, null, vm);
        }

        public async Task<bool> HasActiveOrderForTableAsync(int tableId)
        {
            var active = await _preOrderRepo.GetActiveByTableIdAsync(tableId);
            return active.Any();
        }

        // ── 共用：依目前未結金額重新判斷活動＆優惠券折扣 ──────────────────────────
        private record DiscountResult(int Amount, HashSet<int> InvalidEventOrderIds, HashSet<int> InvalidCouponOrderIds);

        private async Task<DiscountResult> ComputeDiscountAsync(List<PreOrder> orders, int unbilledAmount)
        {
            int total = 0;
            bool eventApplied = false;
            var invalidEvents  = new HashSet<int>();
            var invalidCoupons = new HashSet<int>();

            foreach (var order in orders)
            {
                int orderDiscount = 0;

                // 活動折扣（非贈品，整桌只套一次，且須達最低消費）
                if (order.EventId.HasValue)
                {
                    var ev = await _eventRepo.GetEditByIdAsync(order.EventId.Value);
                    if (ev != null && ev.DiscountType != "Gift")
                    {
                        if (!eventApplied && ev.MinSpend <= unbilledAmount)
                        {
                            orderDiscount += ev.DiscountType == "Percent"
                                ? (int)(unbilledAmount * ev.DiscountValue / 100m)
                                : (int)ev.DiscountValue;
                            eventApplied = true;
                        }
                        else
                        {
                            invalidEvents.Add(order.Id);
                        }
                    }
                }

                // 優惠券折扣（須達最低消費）
                if (order.CouponId.HasValue && order.Coupon != null)
                {
                    if (order.Coupon.MinSpend <= unbilledAmount)
                    {
                        orderDiscount += order.Coupon.DiscountType == 0
                            ? order.Coupon.DiscountValue
                            : (int)(unbilledAmount * order.Coupon.DiscountValue / 100m);
                    }
                    else
                    {
                        invalidCoupons.Add(order.Id);
                    }
                }

                total += orderDiscount;
            }

            return new DiscountResult(Math.Min(total, unbilledAmount), invalidEvents, invalidCoupons);
        }
    }
}