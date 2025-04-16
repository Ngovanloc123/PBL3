// using StackBook.Models;
// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;

// namespace StackBook.DAL.IRepository
// {
//     public interface ICartRepository
//     {
//         // Lấy cart theo user, nếu chưa có thì null
//         Task<Cart?> GetByUserIdAsync(Guid userId);

//         // Lấy cart, nếu chưa có thì tạo mới
//         Task<Cart> GetOrCreateByUserIdAsync(Guid userId);

//         // Lấy chi tiết giỏ hàng
//         Task<List<CartDetail>> GetCartDetailsAsync(Guid userId);

//         // Thêm mới cart (thường dùng khi khởi tạo lần đầu)
//         void AddCart(Cart cart);

//         // Cập nhật cart (ví dụ cập nhật CreatedCart hoặc thông tin chung)
//         void UpdateCart(Cart cart);

//         // Xóa cart (nếu có nhu cầu)
//         void RemoveCart(Cart cart);

//         // Thêm chi tiết (CartDetail)
//         void AddDetail(CartDetail detail);

//         // Cập nhật chi tiết
//         void UpdateDetail(CartDetail detail);

//         // Xóa chi tiết
//         void RemoveDetail(CartDetail detail);

//         // Ghi vào DB
//         Task SaveChangesAsync();
//     }
// }
