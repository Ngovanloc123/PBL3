using Microsoft.Identity.Client;
using StackBook.Data;
using StackBook.Models;
using StackBook.ViewModels;
using StackBook.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace StackBook.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;

        private readonly IAuthorService _authorService;

        public BookService(ApplicationDbContext context, IAuthorService authorService)
        {
            _context = context;
            _authorService = authorService;
        }

        public List<BookWithAuthors> GetListBookAuthorsByCategoryId(Guid categoryId)
        {
            //return _context.BookCategories
            //    .Where(cb => cb.CategoryId == categoryId)
            //    .Select(cb => new BookWithAuthors
            //    {
            //        Book = cb.Book,
            //        Authors = _authorService.GetAuthorsByBookId(cb.BookId)
            //    })
            //    .ToList();
            return null;
        }
        public async Task UpdateBookQuantity(Guid bookId, int quantity, int status)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == bookId);
            if (book != null)
            {
                if (status == 1)
                {
                    if (book.Stock >= quantity)
                    {
                        book.Stock -= quantity;
                    }
                    else
                    {
                        throw new InvalidOperationException("Not enough stock available.");
                    }
                }
                else if (status == 0)
                {
                    book.Stock += quantity;
                }
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Book> GetByIdAsync(Guid bookId)
        {
            var book = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b => b.BookId == bookId);
            if (book == null)
            {
                throw new KeyNotFoundException("Book not found.");
            }
            return book;
        }
        public async Task UpdateAsync(Book book)
        {
            var existingBook = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b => b.BookId == book.BookId);
            if (existingBook == null)
            {
                throw new KeyNotFoundException("Book not found.");
            }
            existingBook.BookTitle = book.BookTitle;
            existingBook.Description = book.Description;
            existingBook.Price = book.Price;
            existingBook.Stock = book.Stock;
            existingBook.ImageURL = book.ImageURL;
            _context.Books.Update(existingBook);
            await _context.SaveChangesAsync();
            // Update authors and categories if needed
        }
        public async Task<List<Book>> GetAllAsync()
        {
            return await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Categories)
                .ToListAsync();
        }
        public async Task<List<Book>> GetBooksByCategoryIdAsync(Guid categoryId)
        {
            return await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Categories)
                .Where(b => b.Categories.Any(c => c.CategoryId == categoryId))
                .ToListAsync();
        }
        public async Task<List<Book>> GetBooksByAuthorIdAsync(Guid authorId)
        {
            return await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Categories)
                .Where(b => b.Authors.Any(a => a.AuthorId == authorId))
                .ToListAsync();
        }
    }
}
