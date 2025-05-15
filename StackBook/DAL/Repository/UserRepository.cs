using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using StackBook.Models;
using StackBook.Data;
using StackBook.DAL.IRepository;
//using StackBook.Interfaces;

namespace StackBook.DAL.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<User> CreateAsync(User entity)
        {
            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<IEnumerable<User>> GetAllAsync(){
            return await _context.Users.ToListAsync();
        }
        public async Task<User> GetByIdAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            return user ?? throw new Exception("User not found");
        }
        public async Task<User> UpdateAsync(User entity)
        {
            _context.Users.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
        {
            return await _context.Users.Where(predicate).ToListAsync();
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<User?> GetUserByGoogleIdAsync(string googleId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<User?> GetUserByResetTokenAsync(string resetToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ResetPasswordToken == resetToken);
        }
        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }
        public async Task CreateGoogleUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        public async Task<User?> GetUserByVerificationTokenAsync(string verificationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == verificationToken);
        }
    }
}