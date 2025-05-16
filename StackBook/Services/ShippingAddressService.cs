using StackBook.DAL.IRepository;
using StackBook.Interfaces;
using StackBook.Models;

namespace StackBook.Services
{
    public class ShippingAddressService : IShippingAddressService
    {
        private readonly IShippingAddressRepository _shippingAddressRepository;
        public ShippingAddressService(IShippingAddressRepository shippingAddressRepository)
        {
            _shippingAddressRepository = shippingAddressRepository;
        }
        public async Task<ShippingAddress> Add(ShippingAddress shippingAddress)
        {
            await _shippingAddressRepository.AddAsync(shippingAddress);
            return shippingAddress;
        }
    }
    
    
}
