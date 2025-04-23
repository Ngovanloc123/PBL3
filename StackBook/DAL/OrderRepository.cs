using Microsoft.EntityFrameworkCore;
using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.Models;

namespace StackBook.DAL
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        // Tạo đơn hàng mới
        public async Task<Order> CreateOrderAsync(Guid userId, Guid discountId, Guid shippingAddressId, double totalPrice, int status)
        {
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                UserId = userId,
                DiscountId = discountId,
                ShippingAddressId = shippingAddressId,
                TotalPrice = totalPrice,
                Status = status
            };

            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();
            return order;
        }

        // Lấy danh sách tất cả đơn hàng của một người dùng
        public async Task<List<Order>> GetAllOrdersAsync(Guid userId)
        {
            return await _db.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
                .ToListAsync();
        }

        // Lấy thông tin đơn hàng theo ID
        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            return await _db.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        // Cập nhật trạng thái đơn hàng
        public async Task<Order> UpdateOrderStatusByIdAsync(Guid orderId, int status)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order == null)
                return null;

            order.Status = status;
            _db.Orders.Update(order);
            await _db.SaveChangesAsync();
            return order;
        }
    }
}
