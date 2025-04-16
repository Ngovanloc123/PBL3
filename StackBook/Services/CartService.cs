// using StackBook.Interfaces;
// using StackBook.Models;
// using StackBook.Data;
// using Microsoft.EntityFrameworkCore;
// using System.Linq.Expressions;
// using StackBook.Exceptions;
// using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
// using StackBook.DAL;

// namespace StackBook.Services
// {
//     public class CartService : ICartService
//     {
//         private readonly ApplicationDbContext _context;
//         public CartService(ApplicationDbContext context)
//         {
//             _context = context;
//         }
//         public async Task<Cart> GetOrCreateCartAsync(Guid userId)
//         {
//             var cart = await _context.Carts.Include(c => c.CartDetails)
//                 .ThenInclude(cd => cd.Book)
//                 .FirstOrDefaultAsync(c => c.UserId == userId);
//             if (cart == null)
//             {
//                 cart = new Cart[[[[[[[[[]]]]]]]]]
//                 {
//                     UserId = userId,
//                     CartDetails = new List<CartDetail>()
//                 };
//                 _context.Carts.Add(cart);
//                 await _context.SaveChangesAsync();
//             }

//             return cart;
//         }

//         public async Task<Cart> GetCartAsync(Guid userId)
//         {
//             var cart = await GetOrCreateCartAsync(userId);
//             foreach (var detail in cart.CartDetails)
//             {
//                 if(detail.Quantity > detail.Book.Stock)
//                 {
//                     throw new OutOfStockException($"Book {detail.Book.BookTitle} is out of stock. Available: {detail.Book.Stock}, Requested: {detail.Quantity}");
//                 }
//             }
//             return cart;
//         }
//         public async Task<List<CartDetail>> GetCartDetailsAsync(Guid userId)
//         {
//             var cart = await GetOrCreateCartAsync(userId);
//             return cart.CartDetails.ToList();
//         }
//         public async Task AddToCartAsync(Guid userId, Guid bookId, int quantity)
//         {
//             var cart = await GetOrCreateCartAsync(userId);
//             var book = await _context.Books.FindAsync(bookId) ?? throw new KeyNotFoundException("Book not found.");

//             var cartDetail = cart.CartDetails.FirstOrDefault(cd => cd.BookId == bookId);
//             var newQty = (cartDetail?.Quantity ?? 0) + quantity;
//             if (newQty > book.Stock)
//             {
//                 throw new OutOfStockException($"Book {book.BookTitle} is out of stock. Available: {book.Stock}, Requested: {newQty}");
//             }
//             if (cartDetail == null)
//             {
//                 cartDetail = new CartDetail
//                 {
//                     BookId = bookId,
//                     Quantity = quantity,
//                     Book = book
//                 };
//                 cart.CartDetails.Add(cartDetail);
//             }
//             else
//             {
//                 cartDetail.Quantity = newQty;
//             }
//             await _context.SaveChangesAsync();
//         }
//         public async Task UpdateCartAsync(Guid userId, Guid bookId, int quantity)
//         {
//             var cart = await GetOrCreateCartAsync(userId);
//             var cartDetail = cart.CartDetails.FirstOrDefault(cd => cd.BookId == bookId) ?? throw new KeyNotFoundException("Book not found in cart.");
//             var book = await _context.Books.FindAsync(bookId) ?? throw new KeyNotFoundException("Book not found.");
//             if(quantity <= 0)
//             {
//                 cart.CartDetails.Remove(cartDetail);
//             }
//             else
//             {
//                 if (quantity > book.Stock)
//                 {
//                     throw new OutOfStockException($"Book {book.BookTitle} is out of stock. Available: {book.Stock}, Requested: {quantity}");
//                 }
//                 cartDetail.Quantity = quantity;
//             }
//             await _context.SaveChangesAsync();
//         }
//         public async Task RemoveFromCartAsync(Guid userId, Guid bookId)
//         {
//             var cart = await GetOrCreateCartAsync(userId);
//             var cartDetail = cart.CartDetails.FirstOrDefault(cd => cd.BookId == bookId) ?? throw new KeyNotFoundException("Book not found in cart.");
//             if(cartDetail != null)
//             {
//                 cart.CartDetails.Remove(cartDetail);
//                 await _context.SaveChangesAsync();
//             }
//         }
//         public async Task ClearCartAsync(Guid userId, bool clearAll = false)
//         {
//             var cart = await GetOrCreateCartAsync(userId);
//             if (clearAll)
//             {
//                 _context.Carts.Remove(cart);
//             }
//             else
//             {
//                 cart.CartDetails.Clear();
//             }
//             await _context.SaveChangesAsync();
//         }
//     }
// }