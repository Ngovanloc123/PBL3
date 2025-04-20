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
            var books = _db.Books
                           .Include(b => b.BookCategories)
                           .Include(b => b.BookAuthors)
                               .ThenInclude(ba => ba.Author)
                           .Where(b => b.BookCategories.Any(bc => bc.CategoryId == categoryId))
                           .ToList();

            var result = books.Select(b => new BookWithAuthors
            {
                Book = b,
                Authors = b.BookAuthors.Select(ba => ba.Author).ToList()
            }).ToList();

            return result;
        }



        public List<CategoryWithBooksViewModel> GetCategoriesWithBooks()
        {
            return _db.Categories
                .Include(c => c.BookCategories)
                    .ThenInclude(cb => cb.Book)
                        .ThenInclude(b => b.BookAuthors)
                            .ThenInclude(ba => ba.Author)
                .Select(c => new CategoryWithBooksViewModel
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    // Sách và những tác gia của sách đó
                    BookWithAuthors = c.BookCategories
                        .Select(cb => new BookWithAuthors
                        {
                            Book = cb.Book,
                            Authors = cb.Book.BookAuthors.Select(ba => ba.Author).ToList()
                        })
                        .ToList()
                })
                .ToList();
        }
    }
}
