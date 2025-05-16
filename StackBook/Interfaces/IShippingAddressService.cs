using StackBook.Models;

namespace StackBook.Interfaces
{
    public interface IShippingAddressService
    {
        Task<ShippingAddress> Add(ShippingAddress shippingAddress);
    }
}
