using Microsoft.Identity.Client;
using StackBook.Data;
using StackBook.Models;

namespace StackBook.Services
{
    public class BookService
    {
        private readonly ApplicationDbContext _context;

        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Book> GetAllBooks()
        {
            return _context.Books.ToList();
        }

        public List<Book> GetBooksByCategoryName(string categoryName)
        {
            return _context.BookCategories
                .Where(bc => bc.Category.CategoryName == categoryName) // Lọc theo tên danh mục
                .Join(_context.Books,
                      bc => bc.BookId,
                      b => b.BookId,
                      (bc, b) => b)
                .ToList();
        }

    }
}
