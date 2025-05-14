using StackBook.Data;
using StackBook.Models;
using StackBook.VMs;

namespace StackBook.Interfaces
{
    public interface ICartService
    {
        Task CreateCartAsync(Guid userId);
        Task AddToCartAsync(Guid userId, Guid bookId, int quantity);
        Task UpdateQuantityAsync(Guid userId, Guid bookId, int quantity);
        Task RemoveFromCartAsync(Guid userId, Guid bookId);
        Task ClearCartAsync(Guid userId);
        Task<Cart> GetCartDetailsAsync(Guid userId);
        Task<double> GetTotalPriceCartAsync(Guid userId);
        Task<int> GetCartCount(Guid userId);
    }
}