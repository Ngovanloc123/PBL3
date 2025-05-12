using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using StackBook.Models;
using StackBook.Data;
using StackBook.DAL.IRepository;
using StackBook.DAL.Repository;
using DocumentFormat.OpenXml.InkML;

namespace StackBook.DAL
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        //
        public async Task<User> CreateAsync(User entity)
        {
            await _db.Users.AddAsync(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _db.Users.ToListAsync();
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            return user ?? throw new Exception("User not found");
        }
        public async Task<User> UpdateAsync(User entity)
        {
            _db.Users.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
        public async Task DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }
        public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
        {
            return await _db.Users.Where(predicate).ToListAsync();
        }
        //public async Task SaveAsync()
        //{
        //    await _context.SaveChangesAsync();
        //}
        public async Task<User?> GetUserByGoogleIdAsync(string googleId)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByResetTokenAsync(string resetToken)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.ResetPasswordToken == resetToken);
        }
        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }
        public async Task CreateGoogleUserAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }
    }
}