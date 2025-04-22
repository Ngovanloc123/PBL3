using StackBook.DTOs;
using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackBook.DAL.IRepository
{
    public interface ICartRepository
    {
        Task<Cart?> GetByUserIdAsync(Guid userId);
        Task<Cart> GetOrCreateByUserIdAsync(Guid userId);
        Task<CartBook?> GetCartBookAsync(Guid cartId, Guid bookId);
        Task AddCartBookAsync(CartBook cartBook);
        Task UpdateCartBookAsync(CartBook cartBook);
        Task RemoveCartBookAsync(CartBook cartBook);
        Task<List<CartBook>> GetCartBooksAsync(Guid cartId);
        Task SaveAsync();
    }
}
