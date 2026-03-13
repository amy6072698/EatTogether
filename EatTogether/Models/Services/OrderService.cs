using EatTogether.Models.EfModels;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using EatTogether.Models.Repositories;
using EatTogether.Models.DTOs;

namespace EatTogether.Models.Services
{
    public interface IOrderService
    {
        // CreatePreOrder
        Task<string> CreatePreOrderAsync(CreatePreOrderDto dto);
        Task<List<SelectListItem>> GetTableOptionsAsync();
        Task<List<CreatePreOrderItemViewModel>> GetMenuItemsAsync();
        // PreOrdersList
        Task<List<PreOrderListItemViewModel>> GetPendingPreOrdersAsync();
        Task UpdatePreOrderDetailStatusAsync(int detailId, int status);

        // Payment
        Task<List<PaymentPreOrderSummaryViewModel>> GetTodayPendingForPaymentAsync();
        Task<PaymentCheckoutViewModel> GetCheckoutDetailAsync(int preOrderId);
        Task CancelUnservedDetailsAsync(int preOrderId);
        Task<int> CheckoutAsync(int preOrderId);
        Task<PaymentIndexViewModel> GetPaymentIndexAsync();
    }
    public class OrderService : IOrderService
    {
        private readonly IPreOrderRepository _preOrderRepo;
        private readonly ITableRepository _tableRepo;
        private readonly IProductRepository _productRepo;
        private readonly IOrderRepository _orderRepo;

        public OrderService(
            IPreOrderRepository preOrderRepo,
            ITableRepository tableRepo,
            IProductRepository productRepo,
            IOrderRepository orderRepo)
        {
            _preOrderRepo = preOrderRepo;
            _tableRepo = tableRepo;
            _productRepo = productRepo;
            _orderRepo = orderRepo;
        }

        // ── CreatePreOrder ──────────────────────────────────────────────────
        public async Task<string> CreatePreOrderAsync(CreatePreOrderDto dto)
        {
            var orderNumber = await GenerateOrderNumberAsync();
            var originalAmount = dto.Items.Sum(i => i.Qty * i.UnitPrice);
            var discountAmount = dto.DiscountAmount;

            var preOrder = new PreOrder
            {
                OrderNumber = orderNumber,
                InOrOut = dto.InOrOut,
                TableId = dto.InOrOut ? dto.TableId : null,
                OrderAt = DateTime.Now,
                OriginalAmount = (int)originalAmount,
                DiscountAmount = discountAmount,
                TotalAmount = (int)(originalAmount - discountAmount),
                Note = dto.Note,
                PayMethod = dto.PayMethod,
                DoneOrCancel = PreOrderStatus.Pending,
                PreOrderDetails = dto.Items.Select(i => new PreOrderDetail
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Qty = i.Qty,
                    UnitPrice = (int)i.UnitPrice,
                    SubTotal = (int)(i.Qty * i.UnitPrice)
                }).ToList()
            };

            await _preOrderRepo.AddAsync(preOrder);
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

        public async Task<List<SelectListItem>> GetTableOptionsAsync()
        {
            var tables = await _tableRepo.GetAllAsync();

            // 找出今日有未完成(Pending)PreOrder的桌號ID
            var today = DateTime.Today;
            var pendingOrders = await _preOrderRepo.GetByStatusAsync(PreOrderStatus.Pending);
            var occupiedTableIds = pendingOrders
                .Where(p => p.InOrOut && p.OrderAt.Date == today && p.TableId.HasValue)
                .Select(p => p.TableId!.Value)
                .ToHashSet();

            return tables
                .Where(t => !occupiedTableIds.Contains(t.Id))
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.TableName
                }).ToList();
        }
        public async Task<List<CreatePreOrderItemViewModel>> GetMenuItemsAsync()
        {
            var products = await _productRepo.GetAllAsync();
            var result = new List<CreatePreOrderItemViewModel>();

            foreach (var p in products)
            {
                if (p.Dish == null) continue;  // Dish 是 null 就跳過

                result.Add(new CreatePreOrderItemViewModel
                {
                    ProductId = p.Id,
                    ProductName = p.Dish.DishName,
                    UnitPrice = (int)p.Dish.Price,
                    Qty = 0
                });
            }
            return result;
        }

        // ── PreOrdersList ──────────────────────────────────────────────────
        public async Task<List<PreOrderListItemViewModel>> GetPendingPreOrdersAsync()
        {
            var list = await _preOrderRepo.GetByStatusAsync(PreOrderStatus.Pending);
            var today = DateTime.Today;

            return list
                .Where(p => p.OrderAt.Date == today)   // ← 補上只顯示當日
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
                        Status = d.DoneOrCancel
                    }).ToList()
                }).ToList();
        }
        
        public async Task UpdatePreOrderDetailStatusAsync(int detailId, int status)
        {
            await _preOrderRepo.UpdateDetailStatusAsync(detailId, status);
        }
        // ── Payment ──────────────────────────────────────────────────
        public async Task<List<PaymentPreOrderSummaryViewModel>> GetTodayPendingForPaymentAsync()
        {
            var today = DateTime.Today;
            var list = await _preOrderRepo.GetByStatusAsync(PreOrderStatus.Pending);
            return list
                .Where(p => p.OrderAt.Date == today)
                .OrderBy(p => p.OrderAt)
                .Select(p => new PaymentPreOrderSummaryViewModel
                {
                    PreOrderId = p.Id,
                    OrderNumber = p.OrderNumber,
                    InOrOut = p.InOrOut,
                    TableName = p.Table?.TableName ?? "外帶",
                    OrderAt = p.OrderAt,
                    TotalAmount = p.TotalAmount,
                    HasUnserved = p.PreOrderDetails.Any(d => d.DoneOrCancel == 0)
                }).ToList();
        }

        public async Task<PaymentCheckoutViewModel> GetCheckoutDetailAsync(int preOrderId)
        {
            var p = await _preOrderRepo.GetByIdAsync(preOrderId);
            if (p == null) return null;

            return new PaymentCheckoutViewModel
            {
                PreOrderId = p.Id,
                OrderNumber = p.OrderNumber,
                InOrOut = p.InOrOut,
                TableName = p.Table?.TableName ?? "外帶",
                PayMethod = p.PayMethod,
                OriginalAmount = p.OriginalAmount,
                DiscountAmount = p.DiscountAmount,
                TotalAmount = p.TotalAmount,
                HasUnserved = p.PreOrderDetails.Any(d => d.DoneOrCancel == 0),
                Items = p.PreOrderDetails.Select(d => new PaymentDetailItemViewModel
                {
                    DetailId = d.Id,
                    ProductName = d.ProductName,
                    Qty = d.Qty,
                    UnitPrice = d.UnitPrice,
                    SubTotal = d.SubTotal,
                    Status = d.DoneOrCancel
                }).ToList()
            };
        }

        public async Task CancelUnservedDetailsAsync(int preOrderId)
        {
            await _preOrderRepo.CancelUnservedDetailsAsync(preOrderId);
        }

        public async Task<int> CheckoutAsync(int preOrderId)
        {
            var preOrder = await _preOrderRepo.GetByIdAsync(preOrderId);

            // 1. 建立 Payment
            var payment = new Payment
            {
                PreOrderId = preOrderId,
                Method = preOrder.PayMethod,
                PaidAt = DateTime.Now,
                DoneOrCancel = 1
            };

            // 2. 建立 Order
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
                PayMethod = preOrder.PayMethod,
                OrderDetails = preOrder.PreOrderDetails
                    .Where(d => d.DoneOrCancel == 1)  // 只存已完成的
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

            // 3. 更新 PreOrder 狀態
            await _preOrderRepo.UpdateStatusAsync(preOrderId, PreOrderStatus.Done);

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
                var order = todayDine.FirstOrDefault(p => p.TableId == t.Id);
                return new TableStatusViewModel
                {
                    TableId = t.Id,
                    TableName = t.TableName,
                    HasOrder = order != null,
                    HasUnserved = order?.PreOrderDetails.Any(d => d.DoneOrCancel == 0) ?? false,
                    PreOrderId = order?.Id,
                    TotalAmount = order?.TotalAmount ?? 0
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
    }
}