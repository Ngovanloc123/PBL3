using Microsoft.EntityFrameworkCore;
using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.Models;

namespace StackBook.DAL
{
    public class OrderDetailsRepository : IOrderDetailsRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderDetailsRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        // Tạo OrderDetail mới
        public async Task<OrderDetail> CreateOrderDetailsAsync(Guid orderId, Guid bookId, int quantity)
        {
            var orderDetail = new OrderDetail
            {
                OrderDetailId = Guid.NewGuid(),
                OrderId = orderId,
                BookId = bookId,
                Quantity = quantity
            };

            await _db.OrderDetails.AddAsync(orderDetail);
            await _db.SaveChangesAsync();
            return orderDetail;
        }

        // Lấy toàn bộ OrderDetails trong một Order
        public async Task<List<OrderDetail>> GetAllOrderDetailsAsync(Guid orderId)
        {
            return await _db.OrderDetails
                .Where(od => od.OrderId == orderId)
                .Include(od => od.Book) // Load thông tin sách nếu có navigation
                .ToListAsync();
        }

        // Lấy một OrderDetail theo Id
        public async Task<OrderDetail?> GetOrderDetailByIdAsync(Guid orderDetailId)
        {
            return await _db.OrderDetails
                .Include(od => od.Book)
                .Include(od => od.Order)
                .FirstOrDefaultAsync(od => od.OrderDetailId == orderDetailId);
        }
    }
}
