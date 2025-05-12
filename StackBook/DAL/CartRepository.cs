using Microsoft.EntityFrameworkCore;
using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.VMs;
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
                .Include(c => c.CartDetails)
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
                    CartDetails = new List<CartDetail>()
                };
                _db.Carts.Add(cart);
                await _db.SaveChangesAsync();
            }
            return cart;
        }

        public async Task<CartDetail?> GetCartBookAsync(Guid cartId, Guid bookId)
        {
            return await _db.CartDetails.FirstOrDefaultAsync(cb => cb.CartId == cartId && cb.BookId == bookId);
        }

        public async Task<List<CartDetail>> GetCartBooksAsync(Guid cartId)
        {
            return await _db.CartDetails.Where(cb => cb.CartId == cartId).ToListAsync();
        }

        public Task AddCartBookAsync(CartDetail cartDetail)
        {
            _db.CartDetails.Add(cartDetail);
            return Task.CompletedTask;
        }
        public Task UpdateCartBookAsync(CartDetail cartDetail)
        {
            _db.CartDetails.Update(cartDetail);
            return Task.CompletedTask;
        }

        public Task RemoveCartBookAsync(CartDetail cartDetail)
        {
            _db.CartDetails.Remove(cartDetail);
            return Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }

}
