using System.Linq.Expressions;
using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.Models;

namespace StackBook.DAL
{
    public class AuthorRepository : Repository<Author>, IAuthorRepository
    {
        private readonly ApplicationDbContext _db;
        public AuthorRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Author obj)
        {
            _db.Authors.Update(obj);
        }
    }
}
