using StackBook.Models;
using StackBook.Services;

namespace StackBook.DAL.IRepository
{
    public interface IOrderRepository
    {
        //Create new Order
        Task<Order> CreateOrderAsync(Guid userId, Guid discountId, Guid shippingAddressId, double totalPrice, int status);

        //Get all Orders
        Task<List<Order>> GetAllOrdersAsync(Guid userId);

        //Get Orser By Id
        Task<Order?> GetOrderByIdAsync(Guid orderId);

        //Update Status Order
        Task<Order> UpdateOrderStatusByIdAsync(Guid orderId, int status);

    }
}
