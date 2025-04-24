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
            //return _context.BookAuthors
            //    .Where(ba => ba.BookId == bookId)
            //    .Select(ba => ba.Author)
            //    .ToList();
            return null;
        }
    }
}
