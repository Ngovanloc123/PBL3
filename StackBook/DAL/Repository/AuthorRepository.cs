using System.Linq.Expressions;
using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.Models;

namespace StackBook.DAL.Repository
{
    public class AuthorRepository : Repository<Author>, IAuthorRepository
    {
        private readonly ApplicationDbContext _db;
        public AuthorRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
