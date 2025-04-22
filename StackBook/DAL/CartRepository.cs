using Microsoft.EntityFrameworkCore;
using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.DTOs;
using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StackBook.DAL
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;

        public CartRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<Cart?> GetByUserIdAsync(Guid userId)
        {
            var cart = await _db.Carts
                .Include(c => c.CartBooks)
                .ThenInclude(cb => cb.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            return cart;
        }
        public async Task<Cart> GetOrCreateByUserIdAsync(Guid userId)
        {
            var cart = await GetByUserIdAsync(userId);
            if(cart != null)
            {
                return cart;
            }
            cart = new Cart
            {
                UserId = userId,
                CartId = Guid.NewGuid(),
                CartBooks = new List<CartBook>()
            };
            _db.Carts.Add(cart);
            await _db.SaveChangesAsync();
            return cart;
        }
        public async Task<List<BookInCartDto>> GetCartDetailsAsync(Guid userId)
        {
            var cart = await _db.Carts
                .Include(c => c.CartBooks)
                .ThenInclude(cb => cb.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.CartBooks == null)
            {
                return new List<BookInCartDto>();
            }

            List<BookInCartDto> books = new List<BookInCartDto>();
            foreach (var detail in cart.CartBooks)
            {
                if (detail.Book != null)
                {
                    books.Add(new BookInCartDto
                    {
                        BookId = detail.BookId,
                        BookTitle = detail.Book.BookTitle,
                        Quantity = detail.Quantity
                    });
                }
            }
            return books;
        }
        public async Task AddDetailAsync(Guid userId, Guid bookId, int quantity)
        {
            // Kiểm tra xem giỏ hàng đã tồn tại chưa
            var cart = await GetOrCreateByUserIdAsync(userId);
            // Kiểm tra xem sách đã có trong giỏ hàng chưa
            var cartBook = await _db.CartBooks.FirstOrDefaultAsync(cb => cb.CartId == cart.CartId && cb.BookId == bookId);
            if (cartBook != null)
            {
                cartBook.Quantity += quantity;
                _db.CartBooks.Update(cartBook);
            }
            else
            {
                cartBook = new CartBook
                {
                    CartId = cart.CartId,
                    BookId = bookId,
                    Quantity = quantity,
                    CreatedCart = DateTime.Now
                };
                _db.CartBooks.Add(cartBook);
            }
            await _db.SaveChangesAsync();
        }
        public async Task UpdateDetailAsync(Guid userId, Guid bookId, int quantity)
        {
            var cart = await GetByUserIdAsync(userId);
            var cartBook = await _db.CartBooks.FirstOrDefaultAsync(cb => cb.CartId == cart.CartId && cb.BookId == bookId);
            if (cartBook != null)
            {
                cartBook.Quantity = quantity;
                _db.CartBooks.Update(cartBook);
                await _db.SaveChangesAsync();
            }   
        }
        public async Task RemoveDetailAsync(Guid userId, Guid bookId)
        {
            var cart = await GetByUserIdAsync(userId);
            var cartBook = await _db.CartBooks.FirstOrDefaultAsync(cb => cb.CartId == cart.CartId && cb.BookId == bookId);
            if (cartBook != null)
            {
                _db.CartBooks.Remove(cartBook);
                await _db.SaveChangesAsync();
            }
        }
        public async Task ClearCartAsync(Guid userId)
        {
            var cart = await GetByUserIdAsync(userId);
            if (cart == null)
            {
                return;
            }
            var cartBooks = await _db.CartBooks.Where(cb => cb.CartId == cart.CartId).ToListAsync();
            if (cartBooks != null && cartBooks.Count > 0)
            {
                _db.CartBooks.RemoveRange(cartBooks);
                await _db.SaveChangesAsync();
            }
        }
    }
}
