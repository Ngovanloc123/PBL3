using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
//using Org.BouncyCastle.Crypto.Generators;
using StackBook.Data;
using StackBook.DTOs;
using StackBook.Models;
using StackBook.Services;
using StackBook.Utils;
using System;
using System.Collections.Generic;
using StackBook.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace StackBook.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> RegisterUser(RegisterDto registerDto)
        {
            if(registerDto == null || string.IsNullOrEmpty(registerDto.Email) || string.IsNullOrEmpty(registerDto.Password) || string.IsNullOrEmpty(registerDto.Username))
            {
                return new BadRequestObjectResult("Invalid data");
            }
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
            if (existingUser != null)
            {
                return new BadRequestObjectResult("Email already exists");
            }
            var hashedPassword = PasswordHashedUtils.HashPassword(registerDto.Password);
            var user = new User
            {
                Email = registerDto.Email,
                FullName = registerDto.Username,
                Password = hashedPassword,
                Role = false,
                CreatedUser = DateTime.Now
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new CreatedAtActionResult(
                "Register successfully",
                "User",
                new { id = user.UserId },
                user
            );
        }

        public async Task<IActionResult> LoginUser(LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password) || string.IsNullOrEmpty(loginDto.Email))
            {
                return new BadRequestObjectResult("Invalid data");
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if(user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.FullName))
            {
                return new UnauthorizedObjectResult("Invalid email or password");
            }
            if (user == null || !PasswordHashedUtils.VerifyPassword(loginDto.Password, user.Password))
            {
                return new UnauthorizedObjectResult("Invalid email or password");
            }
            return new OkObjectResult(user);
        }

        public async Task<IActionResult> UpdateUser(UpdateDto updateDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == updateDto.UserId);

            if (user == null)
            {
                return new NotFoundObjectResult("User not found");
            }

            var email = await _context.Users.FirstOrDefaultAsync(u => u.Email == updateDto.Email);

            if(updateDto.Username != null)
                user.FullName = updateDto.Username;
            if(updateDto.Email != null && email == null)    
                user.Email = updateDto.Email;
            else 
                return new BadRequestObjectResult("Email already exists");
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return new OkObjectResult(user);
        }

        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return new NotFoundObjectResult("User not found");
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return new OkObjectResult(user);
        }

        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return new OkObjectResult(users);
        }

        public async Task<IActionResult> GetUserByName(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FullName == username);
            if (user == null)
            {
                return new NotFoundObjectResult("User not found");
            }
            return new OkObjectResult(user);
        }

        public async Task<IActionResult> UpdatePassword(UpdatePasswordDto updatePasswordDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == updatePasswordDto.UserId);
            if (user == null)
            {
                return new NotFoundObjectResult("User not found");
            }
            if(updatePasswordDto == null || string.IsNullOrEmpty(updatePasswordDto.Password))
            {
                return new BadRequestObjectResult("Invalid data");
            }
            var hashedPassword = PasswordHashedUtils.HashPassword(updatePasswordDto.Password);
            user.Password = hashedPassword;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return new OkObjectResult(user);
        }
    }
}