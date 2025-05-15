using Microsoft.Identity.Client;
using StackBook.Data;
using StackBook.Models;
using StackBook.ViewModels;
using StackBook.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace StackBook.Services
{
    public class BookService: IBookService
    {
        private readonly ApplicationDbContext _context;

        private readonly AuthorService _authorService;

        public BookService(ApplicationDbContext context, AuthorService authorService)
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
        public async Task UpdateBookQuantity(Guid bookId, int quantity, string status)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == bookId);
            if (book != null)
            {
                if(status == "pending")
                {
                    if(book.Stock >= quantity)
                    {
                        book.Stock -= quantity;
                    }
                    else
                    {
                        throw new InvalidOperationException("Not enough stock available.");
                    }
                }
                else if (status == "returned")
                {
                    book.Stock += quantity;
                }
                else if (status == "canceled")
                {
                    book.Stock += quantity;
                }
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
            }
        }
    }
}
