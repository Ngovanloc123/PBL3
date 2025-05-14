using System.Linq.Expressions;
using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.Models;

namespace StackBook.DAL.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
