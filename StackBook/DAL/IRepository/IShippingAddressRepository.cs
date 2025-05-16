using StackBook.Models;
using System;
using System.Threading.Tasks;

namespace StackBook.DAL.IRepository
{
    public interface IShippingAddressRepository : IRepository<ShippingAddress>
    {
        Task<ShippingAddress?> GetByIdAsync(Guid shippingAddressId);
        Task AddAsync(ShippingAddress shippingAddress);
        Task UpdateAsync(ShippingAddress shippingAddress);
        Task DeleteAsync(ShippingAddress shippingAddress);
        Task SaveChangesAsync();
    }
}