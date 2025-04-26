
using StackBook.Models;
using StackBook.Services;

namespace StackBook.DAL.IRepository
{
    public interface ICategoryRepository :IRepository<Category>
    {
        void Update(Category obj);
        void Delete(Category obj);
    }
}
