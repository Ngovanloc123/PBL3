using System.Linq.Expressions;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.Models;
using StackBook.ViewModels;

namespace StackBook.DAL
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Category obj)
        {
            _db.Categories.Update(obj);
        }

        public List<BookWithAuthors> GetBooksByCategoryId(Guid? categoryId)
        {
            if (categoryId == null) return new List<BookWithAuthors>();

            var books = _db.Books
                .Include(b => b.Authors)
                .Include(b => b.Categories)
                .Where(b => b.Categories.Any(c => c.CategoryId == categoryId))
                .ToList();

            var result = books.Select(b => new BookWithAuthors
            {
                Book = b,
                Authors = b.Authors.ToList()
            }).ToList();

            return result;
        }




        public List<CategoryWithBooksViewModel> GetCategoriesWithBooks()
        {
            var categories = _db.Categories
                .Include(c => c.Books)
                    .ThenInclude(b => b.Authors)
                .ToList();

            var result = categories.Select(c => new CategoryWithBooksViewModel
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                BookWithAuthors = c.Books.Select(b => new BookWithAuthors
                {
                    Book = b,
                    Authors = b.Authors.ToList()
                }).ToList()
            }).ToList();

            return result;
        }

    }
}
