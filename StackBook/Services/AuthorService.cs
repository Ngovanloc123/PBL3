using StackBook.Data;
using StackBook.Models;

namespace StackBook.Services
{
    public class AuthorService
    {
        private readonly ApplicationDbContext _context;

        public AuthorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Author> GetAuthorsByBookId(Guid bookId)
        {
            return _context.Authors
                .Where(a => a.Books.Any(b => b.BookId == bookId))
                .ToList();
        }
    }
}
