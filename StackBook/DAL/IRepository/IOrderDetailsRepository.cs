using StackBook.Models;
using StackBook.Services;

namespace StackBook.DAL.IRepository
{
    public interface IOrderDetailsRepository
    {
        //Create new OrderDetails
        Task<OrderDetail> CreateOrderDetailsAsync(Guid orderId, Guid bookId, int quantity);

        //Get all OrderDetails in Order by orderId
        Task<List<OrderDetail>> GetAllOrderDetailsAsync(Guid orderId);

        //Get OrderDetail by orderDetailId
        Task<OrderDetail?> GetOrderDetailByIdAsync (Guid orderDetailId);
    }
}
