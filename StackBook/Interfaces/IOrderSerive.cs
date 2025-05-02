using StackBook.Data;
using StackBook.Models;
using StackBook.DTOs;

namespace StackBook.Interfaces
{
    public interface IOrderService
    {
        Task CreateOrderAsync(Guid userId, Guid discountId, Guid shippingAddressId);

        Task UpdateOrderStatusAsync(int Status, Guid orderId);
    }
}