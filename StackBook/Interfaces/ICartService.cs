using StackBook.Data;
using StackBook.Models;
using StackBook.DTOs;

namespace StackBook.Interfaces
{
    public interface ICartService
    {
        Task AddToCartAsync(Guid userId, Guid bookId, int quantity);
        Task UpdateQuantityAsync(Guid userId, Guid bookId, int quantity);
        Task RemoveFromCartAsync(Guid userId, Guid bookId);
        Task ClearCartAsync(Guid userId);
        Task<List<BookInCartDto>> GetCartDetailsAsync(Guid userId);
        Task<double> GetTotalPriceCartAsync(Guid userId);
    }
}