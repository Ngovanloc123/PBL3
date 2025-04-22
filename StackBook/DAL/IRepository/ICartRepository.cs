using StackBook.DTOs;
using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackBook.DAL.IRepository
{
    public interface ICartRepository
    {
        // Lấy cart theo user, nếu chưa có thì null
        Task<Cart?> GetByUserIdAsync(Guid userId);

        // Lấy cart, nếu chưa có thì tạo mới
        Task<Cart> GetOrCreateByUserIdAsync(Guid userId);

        // Lấy chi tiết giỏ hàng
        Task<List<BookInCartDto>> GetCartDetailsAsync(Guid cartId);

        // Thêm chi tiết (CartDetail)
        Task AddDetailAsync(Guid userId, Guid bookId, int quantity);

        // Cập nhật chi tiết
        Task UpdateDetailAsync(Guid userId, Guid bookId, int quantity);

        // Xóa chi tiết
        Task RemoveDetailAsync(Guid userId, Guid bookId);

        // Clear cart
        Task ClearCartAsync(Guid userId);
    }
}
