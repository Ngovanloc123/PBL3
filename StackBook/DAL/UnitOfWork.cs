using StackBook.Data;
using StackBook.DAL.IRepository;
using StackBook.DAL;
namespace StackBook.DAL.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public ICategoryRepository Category {  get; private set; }
        public IAuthorRepository Author { get; private set; }
        public IBookDetailRepository BookDetail {  get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Author = new AuthorRepository(_db);
            BookDetail = new BookDetailRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
