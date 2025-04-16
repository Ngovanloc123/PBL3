// using Microsoft.EntityFrameworkCore;
// using StackBook.DAL.IRepository;
// using StackBook.Data;
// using StackBook.Models;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;

// namespace StackBook.DAL
// {
//     public class CartRepository : ICartRepository
//     {
//         private readonly ApplicationDbContext _db;

//         public CartRepository(ApplicationDbContext db)
//         {
//             _db = db;
//         }

//         public async Task<Cart?> GetByUserIdAsync(Guid userId)
//         {
//             return await _db.Carts
//                 .Include(c => c.CartDetails)
//                     .ThenInclude(d => d.Book)
//                 .FirstOrDefaultAsync(c => c.UserId == userId);
//         }

//         public async Task<Cart> GetOrCreateByUserIdAsync(Guid userId)
//         {
//             var cart = await GetByUserIdAsync(userId);
//             if (cart == null)
//             {
//                 cart = new Cart { UserId = userId };
//                 AddCart(cart);
//                 await SaveChangesAsync();
//                 // reload để có navigation
//                 cart = await GetByUserIdAsync(userId)!;
//             }
//             return cart;
//         }

//         public async Task<List<CartDetail>> GetCartDetailsAsync(Guid userId)
//         {
//             var cart = await GetOrCreateByUserIdAsync(userId);
//             return cart.CartDetails.ToList();
//         }

//         public void AddCart(Cart cart)
//         {
//             _db.Carts.Add(cart);
//         }

//         public void UpdateCart(Cart cart)
//         {
//             _db.Carts.Update(cart);
//         }

//         public void RemoveCart(Cart cart)
//         {
//             _db.Carts.Remove(cart);
//         }

//         public void AddDetail(CartDetail detail)
//         {
//             _db.CartDetails.Add(detail);
//         }

//         public void UpdateDetail(CartDetail detail)
//         {
//             _db.CartDetails.Update(detail);
//         }

//         public void RemoveDetail(CartDetail detail)
//         {
//             _db.CartDetails.Remove(detail);
//         }

//         public async Task SaveChangesAsync()
//         {
//             await _db.SaveChangesAsync();
//         }
//     }
// }
