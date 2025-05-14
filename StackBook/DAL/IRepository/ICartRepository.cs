using StackBook.VMs;
using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackBook.DAL.IRepository
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task<Cart?> GetByUserIdAsync(Guid userId);
        Task<Cart> GetOrCreateByUserIdAsync(Guid userId);
        Task<CartDetail?> GetCartBookAsync(Guid cartId, Guid bookId);
        Task AddCartBookAsync(CartDetail cartDetail);
        Task UpdateCartBookAsync(CartDetail cartDetail);
        Task RemoveCartBookAsync(CartDetail cartDetail);
        Task<List<CartDetail>> GetCartBooksAsync(Guid cartId);
        Task SaveAsync();
    }
}
