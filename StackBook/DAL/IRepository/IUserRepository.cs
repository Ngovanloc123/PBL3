using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using StackBook.Models;

namespace StackBook.DAL.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> CreateAsync(User entity);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(Guid id);
        Task<User> UpdateAsync(User entity);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate);
        //Task SaveAsync();
        Task<User?> GetUserByGoogleIdAsync(string googleId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByResetTokenAsync(string username);
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
        Task CreateGoogleUserAsync(User user);
    }
}
