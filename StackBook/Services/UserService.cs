using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using StackBook.Data;
using StackBook.Dto;
using StackBook.Models;
using StackBook.Services;

namespace StackBook.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterUser(RegisterDto model)
        {
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                return false; // Email exits
            }
            //hash password
            var PasswordService = new PasswordService();
            var hashedPassword = await PasswordService.HashPassword(model.Password);
            Console.WriteLine(hashedPassword);
            var user = new User
            {
                FullName = model.Username,
                Email = model.Email,
                Password = model.Password, // Chưa mã hóa mật khẩu
                Role = false,
                CreatedUser = new DateTime()
            };
            Console.WriteLine(user);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            Console.WriteLine("Sucess: Create a new user");
            return true;
        }
        public async Task<bool> SiginUser(LoginDto model)
        {
            Console.WriteLine("Checking Signin for: " + model.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                Console.WriteLine("User not found!");
                return false;
            }
            Console.WriteLine("User found: " + user.Email);
            if (user.Password != model.Password)
            {
                Console.WriteLine("Incorrect password!");
                return false;
            }
            return true;
        }
    }
}
