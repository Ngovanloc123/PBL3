
using StackBook.Models;
using StackBook.Services;
using StackBook.ViewModels;

namespace StackBook.DAL.IRepository
{
    public interface ICategoryRepository :IRepository<Category>
    {
        void Update(Category obj);
        List<BookWithAuthors> GetBooksByCategoryId(Guid? categoryId);
        List<CategoryWithBooksViewModel> GetCategoriesWithBooks();
    }
}
