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
            return await _db.Carts
                .Include(c => c.CartBooks)
                .ThenInclude(cb => cb.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart> GetOrCreateByUserIdAsync(Guid userId)
        {
            var cart = await GetByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartId = Guid.NewGuid(),
                    CartBooks = new List<CartBook>()
                };
                _db.Carts.Add(cart);
                await _db.SaveChangesAsync();
            }
            return cart;
        }

        public async Task<CartBook?> GetCartBookAsync(Guid cartId, Guid bookId)
        {
            return await _db.CartBooks.FirstOrDefaultAsync(cb => cb.CartId == cartId && cb.BookId == bookId);
        }

        public async Task<List<CartBook>> GetCartBooksAsync(Guid cartId)
        {
            return await _db.CartBooks.Where(cb => cb.CartId == cartId).ToListAsync();
        }

        public Task AddCartBookAsync(CartBook cartBook)
        {
            _db.CartBooks.Add(cartBook);
            return Task.CompletedTask;
        }

        public Task UpdateCartBookAsync(CartBook cartBook)
        {
            _db.CartBooks.Update(cartBook);
            return Task.CompletedTask;
        }

        public Task RemoveCartBookAsync(CartBook cartBook)
        {
            _db.CartBooks.Remove(cartBook);
            return Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }

}
