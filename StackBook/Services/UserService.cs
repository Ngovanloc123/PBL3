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
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using StackBook.Configurations;
using DocumentFormat.OpenXml.Office.CoverPageProps;
namespace StackBook.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly EMailUtils _emailUtils;
        public UserService(ApplicationDbContext context, EMailUtils eMailUtils)
        {
            _context = context;
            _emailUtils = eMailUtils;
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
            // Send verification email
            string verificationToken = Guid.NewGuid().ToString();
            var user = new User
            {
                Email = registerDto.Email,
                FullName = registerDto.Username,
                Password = hashedPassword,
                Role = false,
                CreatedUser = DateTime.Now,
                IsEmailVerified = false,
                VerificationToken = verificationToken,
                EmailVerifiedAt = DateTime.Now,
                ResetPasswordToken = null,
                ResetTokenExpiry = null,
                LockStatus = false,
                DateLock = DateTime.Now,
                AmountOfTime = 0
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            // Send email with verification link
            var verificationLink = $"http://localhost:5572/auth/verify-email?token={verificationToken}";
            var subject = "Email Verification";
            var message = $"Please verify your email by clicking this link: {verificationLink}";
            await _emailUtils.SendEmailAsync(registerDto.Email, subject, message);
            return new CreatedAtActionResult(
                "Please verify your email",
                "User",
                new { id = user.UserId },
                user
            );
        }

        public async Task<IActionResult> LoginUser(LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
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
            var emailOwner = await _context.Users.FirstOrDefaultAsync(u => u.Email == updateDto.Email && u.UserId != updateDto.UserId);
            if(updateDto.Username != null && emailOwner == null)
                user.FullName = updateDto.Username;
            if(updateDto.Email != null &&  emailOwner == null)    
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

        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == forgotPasswordDto.Email);
            if (user == null)
            {
                return new NotFoundObjectResult("User not found");
            }
            string resetToken = Guid.NewGuid().ToString();
            user.ResetPasswordToken = resetToken;
            user.ResetTokenExpiry = DateTime.Now.AddHours(1);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            var resetLink = $"http://localhost:5572/auth/reset-password?token={resetToken}";
            var subject = "Reset Password";
            var message = $"Please reset your password by clicking this link: {resetLink}";
            if(string.IsNullOrEmpty(forgotPasswordDto.Email) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(message))
            {
                return new BadRequestObjectResult("Invalid data");
            }
            // Send email with reset password link
            await _emailUtils.SendEmailAsync(forgotPasswordDto.Email, subject, message);
            return new OkObjectResult("Reset password email sent");
        }

        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ResetPasswordToken == resetPasswordDto.Token && u.ResetTokenExpiry > DateTime.Now);
            if (user == null)
            {
                return new NotFoundObjectResult("Invalid or expired token");
            }
            if(resetPasswordDto == null || string.IsNullOrEmpty(resetPasswordDto.NewPassword) || string.IsNullOrEmpty(resetPasswordDto.ConfirmPassword))
            {
                return new BadRequestObjectResult("Invalid data");
            }
            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
            {
                return new BadRequestObjectResult("Passwords do not match");
            }
            var hashedPassword = PasswordHashedUtils.HashPassword(resetPasswordDto.NewPassword);
            user.Password = hashedPassword;
            user.ResetPasswordToken = null;
            user.ResetTokenExpiry = null;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return new OkObjectResult("Password reset successfully");
        }
        public async Task<IActionResult> LockUser(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return new NotFoundObjectResult("User not found");
            }
            user.LockStatus = true;
            user.DateLock = DateTime.Now;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return new OkObjectResult("User locked successfully");
        }
        public async Task<IActionResult> UnlockUser(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return new NotFoundObjectResult("User not found");
            }
            user.LockStatus = false;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return new OkObjectResult("User unlocked successfully");
        }
        public async Task<IActionResult> VerifyUser(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsEmailVerified == false);
            if (user == null)
            {
                return new NotFoundObjectResult("User not found or already verified");
            }
            string verificationToken = Guid.NewGuid().ToString();
            user.VerificationToken = verificationToken;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            var verificationLink = $"http://localhost:5572/auth/verify-email?token={verificationToken}";
            var subject = "Email Verification";
            var message = $"Please verify your email by clicking this link: {verificationLink}";
            await _emailUtils.SendEmailAsync(email, subject, message);
            return new OkObjectResult("Verification email sent");
        }
        public async Task<IActionResult> ResendVerificationEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsEmailVerified == false);
            if (user == null)
            {
                return new NotFoundObjectResult("User not found or already verified");
            }
            string verificationToken = Guid.NewGuid().ToString();
            user.VerificationToken = verificationToken;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            var verificationLink = $"http://localhost:5572/auth/verify-email?token={verificationToken}";
            var subject = "Email Verification";
            var message = $"Please verify your email by clicking this link: {verificationLink}";
            await _emailUtils.SendEmailAsync(email, subject, message);
            return new OkObjectResult("Verification email resent");
        }
        public async Task<IActionResult> VerifyEmail(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token && u.IsEmailVerified == false);
            if (user == null)
            {
                return new NotFoundObjectResult("Invalid or expired token");
            }
            user.IsEmailVerified = true;
            user.VerificationToken = null;
            user.EmailVerifiedAt = DateTime.Now;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return new OkObjectResult("Email verified successfully");
        }

        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return new NotFoundObjectResult("User not found");
            }
            return new OkObjectResult(user);
        }
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return new NotFoundObjectResult("User not found");
            }
            return new OkObjectResult(user);
        }
    }
}