using StackBook.Interfaces;
using StackBook.Models;
using StackBook.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using StackBook.Exceptions;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using StackBook.DAL.IRepository;
using StackBook.VMs;

namespace StackBook.Services
{
   public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task CreateCartAsync(Guid userId)
        {
            try
            {
                var cart = await _cartRepository.GetByUserIdAsync(userId);
                if (cart != null) throw new AppException("Giỏ hàng đã tồn tại.");

                var newCart = new Cart
                {
                    CartId = Guid.NewGuid(),
                    UserId = userId,
                };

                await _cartRepository.GetOrCreateByUserIdAsync(userId);
                await _cartRepository.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new AppException($"Lỗi khi tạo giỏ hàng: {ex.Message}");
            }
        }
        public async Task AdVMCartAsync(Guid userId, Guid bookId, int quantity)
        {
            try
            {
                var cart = await _cartRepository.GetOrCreateByUserIdAsync(userId);
                var cartBook = await _cartRepository.GetCartBookAsync(cart.CartId, bookId);

                if (cartBook != null)
                {
                    cartBook.Quantity += quantity;
                    await _cartRepository.UpdateCartBookAsync(cartBook);
                }
                else
                {
                    var newCartBook = new CartDetail
                    {
                        CartId = cart.CartId,
                        BookId = bookId,
                        Quantity = quantity,
                        CreatedCart = DateTime.Now
                    };
                    await _cartRepository.AddCartBookAsync(newCartBook);
                }

                await _cartRepository.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new AppException($"Lỗi khi thêm sách vào giỏ hàng: {ex.Message}");
            }
        }

        public async Task UpdateQuantityAsync(Guid userId, Guid bookId, int quantity)
        {
            try
            {
                var cart = await _cartRepository.GetByUserIdAsync(userId);
                if (cart == null) throw new AppException("Giỏ hàng không tồn tại.");

                var cartBook = await _cartRepository.GetCartBookAsync(cart.CartId, bookId);
                if (cartBook == null) throw new AppException("Sách không có trong giỏ hàng.");

                cartBook.Quantity = quantity;
                await _cartRepository.UpdateCartBookAsync(cartBook);
                await _cartRepository.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new AppException($"Lỗi khi cập nhật số lượng: {ex.Message}");
            }
        }

        public async Task RemoveFromCartAsync(Guid userId, Guid bookId)
        {
            try
            {
                var cart = await _cartRepository.GetByUserIdAsync(userId);
                if (cart == null) throw new AppException("Giỏ hàng không tồn tại.");

                var cartBook = await _cartRepository.GetCartBookAsync(cart.CartId, bookId);
                if (cartBook == null) throw new AppException("Không tìm thấy sách trong giỏ hàng.");

                await _cartRepository.RemoveCartBookAsync(cartBook);
                await _cartRepository.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new AppException($"Lỗi khi xoá sách khỏi giỏ hàng: {ex.Message}");
            }
        }

        public async Task ClearCartAsync(Guid userId)
        {
            try
            {
                var cart = await _cartRepository.GetByUserIdAsync(userId);
                if (cart == null) throw new AppException("Giỏ hàng không tồn tại.");

                var cartBooks = await _cartRepository.GetCartBooksAsync(cart.CartId);
                foreach (var cartBook in cartBooks)
                {
                    await _cartRepository.RemoveCartBookAsync(cartBook);
                }

                await _cartRepository.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new AppException($"Lỗi khi xoá toàn bộ giỏ hàng: {ex.Message}");
            }
        }

        public async Task<List<BookInCartVM>> GetCartDetailsAsync(Guid userId)
        {
            try
            {
                var cart = await _cartRepository.GetByUserIdAsync(userId);
                if (cart == null || cart.CartDetails == null)
                    return new List<BookInCartVM>();

                var result = new List<BookInCartVM>();
                foreach (var cartBook in cart.CartDetails)
                {
                    if (cartBook.Book != null)
                    {
                        var bookVM = new BookInCartVM
                        {
                            BookId = cartBook.BookId,
                            BookTitle = cartBook.Book.BookTitle,
                            Quantity = cartBook.Quantity
                        };
                        result.Add(bookVM);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new AppException($"Lỗi khi lấy chi tiết giỏ hàng: {ex.Message}");
            }
        }
    
            public async Task<double> GetTotalPriceCartAsync(Guid userId)
            {
                try
                {
                    var cart = await _cartRepository.GetByUserIdAsync(userId);
                    if (cart == null || cart.CartDetails == null)
                        return 0;
    
                    double totalPrice = 0;
                    foreach (var cartBook in cart.CartDetails)
                    {
                        if (cartBook.Book != null)
                        {
                            totalPrice += cartBook.Quantity * cartBook.Book.Price;
                        }
                    }
    
                    return totalPrice;
                }
                catch (Exception ex)
                {
                    throw new AppException($"Lỗi khi tính tổng giá trị giỏ hàng: {ex.Message}");
                }
            }
        }
}