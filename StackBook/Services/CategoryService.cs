using Microsoft.EntityFrameworkCore;
using StackBook.Data;
using StackBook.Models;
using StackBook.ViewModels;

namespace StackBook.Services
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthorService _authorService;

        public CategoryService(ApplicationDbContext context , AuthorService authorService)
        {
            _context = context;
            _authorService = authorService;
        }

        // Lấy hết thông tin của Category
        public List<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        // Lấy tên và số lượng sách trong category đó
        public List<MenuCategories> GetMenuCategories()
        {
            var menuCategories = _context.Categories
                .Select(category => new MenuCategories
                {
                    CategoryName = category.CategoryName,
                    Count = category.BookCategories.Count() // Đếm số lượng sách qua bảng trung gian
                })
                .ToList();

            return menuCategories;
        }

        public List<CategoryWithBooksViewModel> GetCategoriesWithBooks()
        {
            return _context.Categories
                .Include(c => c.BookCategories)
                    .ThenInclude(cb => cb.Book)
                        .ThenInclude(b => b.BookAuthors)
                            .ThenInclude(ba => ba.Author)
                .Select(c => new CategoryWithBooksViewModel
                {
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
