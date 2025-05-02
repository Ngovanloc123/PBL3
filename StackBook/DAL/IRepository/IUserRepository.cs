using System.Linq.Expressions;
using StackBook.Models;
using StackBook.Services;

namespace StackBook.DAL.IRepository
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(Guid userId);
    }
}
