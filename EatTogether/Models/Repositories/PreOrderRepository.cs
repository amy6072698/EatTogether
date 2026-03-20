using EatTogether.Models.EfModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace EatTogether.Models.Repositories
{
    public interface IPreOrderRepository
    {
        // Create
        Task<List<PreOrder>> GetByStatusAsync(int doneOrCancel); // string → int
        Task AddAsync(PreOrder preOrder);
        Task<int> CountTodayAsync(DateTime date);
        Task<List<PreOrder>> GetActiveByTableIdAsync(int tableId);
        Task CancelAllByTableIdAsync(int tableId);
        Task SaveChangesAsync();

        // List
        Task UpdateDetailStatusAsync(int detailId, int status);
        Task<List<PreOrder>> GetAllAsync();
        Task<int> GetPreOrderIdByDetailIdAsync(int detailId);
        Task UpdateStatusAsync(int id, int doneOrCancel);

        // Payment
        Task CancelUnservedDetailsAsync(int preOrderId, ISet<int>? excludeDetailIds = null);
        Task<PreOrder?> GetByIdAsync(int id);
        Task CancelEntireOrderAsync(int preOrderId);
        Task UpdateDetailBilledAsync(int detailId);
        Task<bool> HasUnbilledDetailsForTableAsync(int tableId);
        Task<bool> AllNonCancelledDetailsBilledAsync(int preOrderId);
        Task UpdateTableAsync(int preOrderId, int? tableId, bool inOrOut);
    }

    public class PreOrderRepository : IPreOrderRepository
    {
        private readonly EatTogetherDBContext _context;
        public PreOrderRepository(EatTogetherDBContext db) => _context = db;

        // Create -----------------------------------------------------------------------------------------
        public async Task<List<PreOrder>> GetByStatusAsync(int doneOrCancel) =>
            await _context.PreOrders
                     .Include(p => p.PreOrderDetails)
                     .Include(p => p.Table)
                     .Include(p => p.Event)
                     .Include(p => p.Coupon)
                     .Where(p => p.DoneOrCancel == doneOrCancel)
                     .ToListAsync();
        public async Task AddAsync(PreOrder preOrder)
        {
            _context.PreOrders.Add(preOrder);
            await _context.SaveChangesAsync();
        }
        public async Task<int> CountTodayAsync(DateTime date)
        {
            return await _context.PreOrders
                .Where(p => p.OrderAt.Date == date.Date)
                .CountAsync();
        }

        public async Task<List<PreOrder>> GetActiveByTableIdAsync(int tableId)
        {
            var today = DateTime.Today;
            return await _context.PreOrders
                .Include(p => p.PreOrderDetails)
                .Where(p => p.TableId == tableId
                         && p.DoneOrCancel == 0
                         && p.OrderAt.Date == today)
                .ToListAsync();
        }

        public async Task CancelAllByTableIdAsync(int tableId)
        {
            var orders = await GetActiveByTableIdAsync(tableId);
            foreach (var order in orders)
            {
                order.DoneOrCancel = 2;
                order.CancelledAt = DateTime.Now;
                order.Note = string.IsNullOrEmpty(order.Note) ? "取消" : order.Note + "／取消";
                foreach (var detail in order.PreOrderDetails)
                    detail.DoneOrCancel = 2;
            }
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        // List -----------------------------------------------------------------------------------------
        public async Task UpdateDetailStatusAsync(int detailId, int status)
        {
            var detail = await _context.PreOrderDetails.FindAsync(detailId);
            if (detail is null) return;

            detail.DoneOrCancel = status;
            await _context.SaveChangesAsync();
        }
        public async Task<List<PreOrder>> GetAllAsync() =>
            await _context.PreOrders
                     .Include(p => p.PreOrderDetails)
                     .Include(p => p.Table)
                     .Include(p => p.Member)
                     .Include(p => p.Coupon)
                     .Include(p => p.Event)
                     .OrderByDescending(p => p.OrderAt)
                     .ToListAsync();
        public async Task<int> GetPreOrderIdByDetailIdAsync(int detailId)
        {
            var detail = await _context.PreOrderDetails.FindAsync(detailId);
            return detail?.PreOrderId ?? 0;
        }
        public async Task UpdateStatusAsync(int id, int doneOrCancel)
        {
            var entity = await _context.PreOrders.FindAsync(id);
            if (entity is null) return;
            entity.DoneOrCancel = doneOrCancel;
            await _context.SaveChangesAsync();
        }

        // Payment -----------------------------------------------------------------------------------------
        public async Task CancelUnservedDetailsAsync(int preOrderId, ISet<int>? excludeDetailIds = null)
        {
            var query = _context.PreOrderDetails
                .Where(d => d.PreOrderId == preOrderId && d.DoneOrCancel == 0);

            if (excludeDetailIds?.Count > 0)
                query = query.Where(d => !excludeDetailIds.Contains(d.Id));

            var details = await query.ToListAsync();

            foreach (var d in details)
                d.DoneOrCancel = 2;

            await _context.SaveChangesAsync();
        }
        public async Task<PreOrder?> GetByIdAsync(int id) =>
            await _context.PreOrders
                     .Include(p => p.PreOrderDetails)
                     .Include(p => p.Table)
                     .Include(p => p.User)
                     .Include(p => p.Member)
                     .Include(p => p.Coupon)
                     .Include(p => p.Event)
                     .Include(p => p.Payments)
                     .FirstOrDefaultAsync(p => p.Id == id);

        public async Task CancelEntireOrderAsync(int preOrderId)
        {
            var order = await _context.PreOrders
                .Include(p => p.PreOrderDetails)
                .FirstOrDefaultAsync(p => p.Id == preOrderId);

            if (order == null) return;

            order.DoneOrCancel = 2; // Cancelled
            order.CancelledAt = DateTime.Now;
            order.Note = string.IsNullOrEmpty(order.Note) ? "取消" : order.Note + "／取消";

            foreach (var detail in order.PreOrderDetails)
            {
                detail.DoneOrCancel = 2; // Cancelled
            }

            await _context.SaveChangesAsync();
        }
        public async Task UpdateDetailBilledAsync(int detailId)
        {
            var detail = await _context.PreOrderDetails.FindAsync(detailId);
            if (detail is null) return;
            detail.IsBilled = true;
            await _context.SaveChangesAsync();
        }
        public async Task<bool> HasUnbilledDetailsForTableAsync(int tableId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            return await _context.PreOrderDetails
                .AnyAsync(d => d.PreOrder.TableId == tableId
                            && d.PreOrder.DoneOrCancel == 0
                            && d.PreOrder.OrderAt >= today
                            && d.PreOrder.OrderAt < tomorrow
                            && !d.IsBilled
                            && d.DoneOrCancel != 2);
        }

        public async Task<bool> AllNonCancelledDetailsBilledAsync(int preOrderId)
        {
            var details = await _context.PreOrderDetails
                .Where(d => d.PreOrderId == preOrderId && d.DoneOrCancel != 2)
                .ToListAsync();

            return details.Any() && details.All(d => d.IsBilled);
        }

        public async Task UpdateTableAsync(int preOrderId, int? tableId, bool inOrOut)
        {
            var order = await _context.PreOrders.FindAsync(preOrderId);
            if (order == null) return;
            order.TableId = tableId;
            order.InOrOut = inOrOut;
            await _context.SaveChangesAsync();
        }
    }
}
