using StackBook.Data;
using StackBook.Models;
using StackBook.DTOs;

namespace StackBook.Interfaces
{
    public interface ICartService
    {
        Task CreateCartAsync(Guid userId);//tao gio hang
        Task AddToCartAsync(Guid userId, Guid bookId, int quantity);//them sach vao gio hang
        Task UpdateQuantityAsync(Guid userId, Guid bookId, int quantity);//cap nhat so luong sach trong gio hang
        Task RemoveFromCartAsync(Guid userId, Guid bookId);//xoa sach ra khoi gio hang
        Task ClearCartAsync(Guid userId);//xoa toan bo gio hang
        Task<List<BookInCartDto>> GetCartDetailsAsync(Guid userId);//lay danh sach sach trong gio hang
        Task<double> GetTotalPriceCartAsync(Guid userId);//tinh tong tien trong gio hang
    }
}