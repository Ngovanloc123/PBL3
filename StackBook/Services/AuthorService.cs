using Microsoft.EntityFrameworkCore;
using StackBook.Data;
using StackBook.Interfaces;
using StackBook.Models;

namespace StackBook.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly ApplicationDbContext _context;

        public AuthorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Author>> GetAuthorsByBookId(Guid bookId)
        {
            return await _context.Authors
                .Where(a => a.Books.Any(b => b.BookId == bookId))
                .ToListAsync();
        }
    }
}
