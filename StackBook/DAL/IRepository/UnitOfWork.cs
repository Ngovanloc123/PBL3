using StackBook.Data;

namespace StackBook.DAL.IRepository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public ICategoryRepository Category {  get; private set; }
        public IBookRepository Book {  get; private set; }
        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Book = new BookRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
