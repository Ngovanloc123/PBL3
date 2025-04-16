// using StackBook.Data;
// using StackBook.Models;

// namespace StackBook.Interfaces
// {
//     public interface ICartService
//     {
//         Task<Cart> GetOrCreateCartAsync(Guid userId);
//         Task AddToCartAsync(Guid userId, Guid bookId, int quantity);
//         Task UpdateCartAsync(Guid userId, Guid bookId, int quantity);
//         Task RemoveFromCartAsync(Guid userId, Guid bookId);
//         Task ClearCartAsync(Guid userId, bool clearAll = false);
//         Task<Cart> GetCartAsync(Guid userId);
//         Task<List<CartDetail>> GetCartDetailsAsync(Guid userId);
//     }
// }