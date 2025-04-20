using Microsoft.Identity.Client;
using StackBook.Data;
using StackBook.Models;
using StackBook.ViewModels;

namespace StackBook.Services
{
    public class BookService
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
            return _context.BookCategories
                .Where(cb => cb.CategoryId == categoryId)
                .Select(cb => new BookWithAuthors
                {
                    Book = cb.Book,
                    Authors = _authorService.GetAuthorsByBookId(cb.BookId)
                })
                .ToList();
        }


    }
}
