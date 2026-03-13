using EatTogether.Models.EfModels;

namespace EatTogether.Models.Repositories
{
    public interface IOrderRepository 
    {
        Task AddWithPaymentAsync(Order order, Payment payment);
    }
    public class OrderRepository : IOrderRepository 
    {
        private readonly EatTogetherDBContext _context;
        public OrderRepository(EatTogetherDBContext db) => _context = db;
        public async Task AddWithPaymentAsync(Order order, Payment payment)
        {
            // 先存 Payment 取得 Id
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // 再存 Order，帶入 PaymentId
            order.PaymentId = payment.Id;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 回填 Payment.OrderId
            payment.OrderId = order.Id;
            await _context.SaveChangesAsync();
        }
    }
}
