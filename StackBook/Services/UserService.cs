using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
//using Org.BouncyCastle.Crypto.Generators;
using StackBook.Data;
using StackBook.DTOs;
using StackBook.Models;
using StackBook.Services;
using StackBook.Interfaces;
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
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly EMailUtils _emailUtils;
        public UserService(ApplicationDbContext context, EMailUtils eMailUtils)
        {
            _context = context;
            _emailUtils = eMailUtils;
        }
        public async Task<ServiceResponse<User>> RegisterUser(RegisterDto registerDto)
        {
            var response = new ServiceResponse<User>();

            if (registerDto == null || string.IsNullOrEmpty(registerDto.Email) 
                || string.IsNullOrEmpty(registerDto.Password) 
                || string.IsNullOrEmpty(registerDto.Username))
            {
                response.Success = false;
                response.Message = "Invalid data";
                return response;
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
            if (existingUser != null)
            {
                response.Success = false;
                response.Message = "Email already exists";
                return response;
            }

            var hashedPassword = PasswordHashedUtils.HashPassword(registerDto.Password);
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

            var verificationLink = $"http://localhost:5572/auth/verify-email?token={verificationToken}";
            var subject = "Email Verification";
            var message = $"Please verify your email by clicking this link: {verificationLink}";

            await _emailUtils.SendEmailAsync(registerDto.Email, subject, message);

            response.Success = true;
            response.Data = user;
            response.Message = "Registration successful. Verification email sent.";

            return response;
        }
        public async Task<ServiceResponse<User>> LoginUser(LoginDto loginDto)
        {
            var response = new ServiceResponse<User>();

            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
            {
                response.Success = false;
                response.Message = "Invalid data";
                return response;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null || string.IsNullOrEmpty(user.Password) || !PasswordHashedUtils.VerifyPassword(loginDto.Password, user.Password))
            {
                response.Success = false;
                response.Message = "Invalid email or password";
                return response;
            }

            response.Success = true;
            response.Data = user;
            response.Message = "Login successful";
            return response;
        }
        public async Task<ServiceResponse<User>> UpdateUser(UpdateDto updateDto)
        {
            var response = new ServiceResponse<User>();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == updateDto.UserId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            // Kiểm tra email đã tồn tại chưa
            if (!string.IsNullOrEmpty(updateDto.Email))
            {
                var emailOwner = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == updateDto.Email && u.UserId != updateDto.UserId);

                if (emailOwner != null)
                {
                    response.Success = false;
                    response.Message = "Email already exists";
                    return response;
                }

                user.Email = updateDto.Email;
            }

            if (!string.IsNullOrEmpty(updateDto.Username))
            {
                user.FullName = updateDto.Username;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            response.Data = user;
            response.Message = "User updated successfully";
            response.Success = true;

            return response;
        }
        public async Task<ServiceResponse<User>> DeleteUser(Guid userId)
        {
            var response = new ServiceResponse<User>();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            response.Data = user;
            response.Message = "User deleted successfully";
            response.Success = true;

            return response;
        }


        public async Task<ServiceResponse<List<User>>> GetAllUsers()
        {
            var response = new ServiceResponse<List<User>>();

            var users = await _context.Users.ToListAsync();
            if (users == null || users.Count == 0)
            {
                response.Success = false;
                response.Message = "No users found";
                return response;
            }

            response.Data = users;
            response.Message = "Users retrieved successfully";
            response.Success = true;

            return response;
        }

        public async Task<ServiceResponse<User>> GetUserByName(string username)
        {
            var response = new ServiceResponse<User>();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.FullName == username);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            response.Data = user;
            response.Message = "User retrieved successfully";
            response.Success = true;

            return response;
        }
        public async Task<ServiceResponse<User>> GetUserById(Guid userId)
        {
            var response = new ServiceResponse<User>();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            response.Data = user;
            return response;
        }

        public async Task<ServiceResponse<User>> GetUserByEmail(string email)
        {
            var response = new ServiceResponse<User>();

            if (string.IsNullOrEmpty(email))
            {
                response.Success = false;
                response.Message = "Email is required";
                return response;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            response.Data = user;
            return response;
        }

        public async Task<ServiceResponse<User>> UpdatePassword(UpdatePasswordDto updatePasswordDto)
        {
            var response = new ServiceResponse<User>();

            if (updatePasswordDto == null || string.IsNullOrEmpty(updatePasswordDto.Password))
            {
                response.Success = false;
                response.Message = "Invalid data";
                return response;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == updatePasswordDto.UserId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            var hashedPassword = PasswordHashedUtils.HashPassword(updatePasswordDto.Password);
            user.Password = hashedPassword;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            response.Data = user;
            response.Message = "Password updated successfully";
            response.Success = true;

            return response;
        }
        public async Task<ServiceResponse<string>> UpdateEmail(Guid userId, string newEmail)
        {
            var response = new ServiceResponse<string>();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            // Kiểm tra email đã tồn tại chưa
            var emailExists = await _context.Users.AnyAsync(u => u.Email == newEmail && u.UserId != userId);
            if (emailExists)
            {
                response.Success = false;
                response.Message = "Email already in use";
                return response;
            }

            // Cập nhật email + yêu cầu xác minh lại
            user.Email = newEmail;
            user.IsEmailVerified = false;
            user.VerificationToken = Guid.NewGuid().ToString();
            user.EmailVerifiedAt = null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Gửi mail xác minh
            var verificationLink = $"http://localhost:5572/auth/verify-email?token={user.VerificationToken}";
            await _emailUtils.SendEmailAsync(newEmail, "Verify your new email", $"Click here to verify: {verificationLink}");

            response.Data = "Verification email sent to new address";
            response.Message = "Email updated. Please verify your new email.";
            return response;
        }

        public async Task<ServiceResponse<string>> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var response = new ServiceResponse<string>();

            if (string.IsNullOrEmpty(forgotPasswordDto.Email))
            {
                response.Success = false;
                response.Message = "Email is required";
                return response;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == forgotPasswordDto.Email);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            string resetToken = Guid.NewGuid().ToString();
            user.ResetPasswordToken = resetToken;
            user.ResetTokenExpiry = DateTime.Now.AddHours(1);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var resetLink = $"http://localhost:5572/auth/reset-password?token={resetToken}";
            var subject = "Reset Password";
            var message = $"Please reset your password by clicking this link: {resetLink}";

            // Send email with reset password link
            await _emailUtils.SendEmailAsync(forgotPasswordDto.Email, subject, message);

            response.Data = "Reset password email sent";
            response.Message = "Reset password email sent successfully";
            response.Success = true;

            return response;
        }

        public async Task<ServiceResponse<string>> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var response = new ServiceResponse<string>();

            // Kiểm tra dữ liệu đầu vào
            if (resetPasswordDto == null || 
                string.IsNullOrEmpty(resetPasswordDto.NewPassword) || 
                string.IsNullOrEmpty(resetPasswordDto.ConfirmPassword) || 
                string.IsNullOrEmpty(resetPasswordDto.Token))
            {
                response.Success = false;
                response.Message = "Invalid data";
                return response;
            }

            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
            {
                response.Success = false;
                response.Message = "Passwords do not match";
                return response;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.ResetPasswordToken == resetPasswordDto.Token &&
                u.ResetTokenExpiry > DateTime.Now);

            if (user == null)
            {
                response.Success = false;
                response.Message = "Invalid or expired token";
                return response;
            }

            // Hash mật khẩu mới và cập nhật
            user.Password = PasswordHashedUtils.HashPassword(resetPasswordDto.NewPassword);
            user.ResetPasswordToken = null;
            user.ResetTokenExpiry = null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            response.Data = "Password reset successfully";
            response.Message = "Password reset successfully";
            response.Success = true;

            return response;
        }

        public async Task<ServiceResponse<string>> LockUser(Guid userId)
        {
            var response = new ServiceResponse<string>();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            if (user.LockStatus)
            {
                response.Success = false;
                response.Message = "User is already locked";
                return response;
            }

            user.LockStatus = true;
            user.DateLock = DateTime.Now;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            response.Data = "User locked successfully";
            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<string>> UnlockUser(Guid userId)
        {
            var response = new ServiceResponse<string>();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            if (!user.LockStatus)
            {
                response.Success = false;
                response.Message = "User is not locked";
                return response;
            }

            user.LockStatus = false;
            user.DateLock = null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            response.Data = "User unlocked successfully";
            response.Success = true;
            return response;
        }
        public async Task<ServiceResponse<string>> VerifyUser(string email)
        {
            var response = new ServiceResponse<string>();

            if (string.IsNullOrEmpty(email))
            {
                response.Success = false;
                response.Message = "Email is required";
                return response;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsEmailVerified);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found or already verified";
                return response;
            }

            string verificationToken = Guid.NewGuid().ToString();
            user.VerificationToken = verificationToken;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var verificationLink = $"http://localhost:5572/auth/verify-email?token={verificationToken}";
            var subject = "Email Verification";
            var message = $"Please verify your email by clicking this link: {verificationLink}";

            await _emailUtils.SendEmailAsync(email, subject, message);

            response.Data = "Verification email sent";
            response.Success = true;
            return response;
        }
        public async Task<ServiceResponse<string>> ResendVerificationEmail(string email)
        {
            var response = new ServiceResponse<string>();

            if (string.IsNullOrEmpty(email))
            {
                response.Success = false;
                response.Message = "Email is required";
                return response;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsEmailVerified);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found or already verified";
                return response;
            }

            string verificationToken = Guid.NewGuid().ToString();
            user.VerificationToken = verificationToken;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var verificationLink = $"http://localhost:5572/auth/verify-email?token={verificationToken}";
            var subject = "Email Verification";
            var message = $"Please verify your email by clicking this link: {verificationLink}";

            await _emailUtils.SendEmailAsync(email, subject, message);

            response.Data = "Verification email resent";
            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<string>> VerifyEmail(string token)
        {
            var response = new ServiceResponse<string>();

            if (string.IsNullOrEmpty(token))
            {
                response.Success = false;
                response.Message = "Token is required";
                return response;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token && !u.IsEmailVerified);
            if (user == null)
            {
                response.Success = false;
                response.Message = "Invalid or expired token";
                return response;
            }

            user.IsEmailVerified = true;
            user.VerificationToken = null;
            user.EmailVerifiedAt = DateTime.Now;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            response.Data = "Email verified successfully";
            response.Success = true;
            return response;
        }
    }
}