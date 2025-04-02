
using StackBook.Models;
using StackBook.Services;
using StackBook.ViewModels;

namespace StackBook.DAL.IRepository
{
    public interface IBookDetailRepository :IRepository<BookDetailViewModel>
    {
        void Update(BookDetailViewModel obj);
    }
}
