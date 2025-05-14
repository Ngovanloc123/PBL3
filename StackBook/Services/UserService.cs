using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using StackBook.Data;
using StackBook.DTOs;
using StackBook.Models;
using StackBook.Services;
using StackBook.Interfaces;
using StackBook.DAL.IRepository;
using StackBook.Utils;
using System;
using System.Collections.Generic;
using StackBook.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using StackBook.Configurations;
using DocumentFormat.OpenXml.Office.CoverPageProps;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace StackBook.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly EMailUtils _emailUtils;
        private readonly OAuthGoogleService _oauthGoogleService;
        private readonly JwtUtils _jwtUtils;
        private readonly CloudinaryUtils _cloudinaryUtils;
        private readonly IUserRepository _userRepository;
        public UserService(ApplicationDbContext context, EMailUtils eMailUtils, OAuthGoogleService oauthGoogleService, IUserRepository userRepository, JwtUtils jwtUtils, CloudinaryUtils cloudinaryUtils)
        {
            _context = context;
            _emailUtils = eMailUtils;
            _oauthGoogleService = oauthGoogleService;
            _userRepository = userRepository;
            _jwtUtils = jwtUtils;
            _cloudinaryUtils = cloudinaryUtils;
        }
        public async Task<ServiceResponse<User>> UpdateUser(UpdateDto updateDto)
        {
            var response = new ServiceResponse<User>();

            var user = await _userRepository.GetByIdAsync(updateDto.UserId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            // Kiểm tra email đã tồn tại chưa
            if (!string.IsNullOrEmpty(updateDto.Email))
            {
                var emailOwner = await _context.Users.FirstOrDefaultAsync(u => u.Email == updateDto.Email && u.UserId != updateDto.UserId);

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
            var accessToken = _jwtUtils.GenerateAccessToken(user);

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveAsync();

            response.Data = user;
            response.Message = "User updated successfully";
            response.Success = true;
            response.AccessToken = accessToken;
            return response;
        }
        public async Task<ServiceResponse<User>> DeleteUser(Guid userId)
        {
            var response = new ServiceResponse<User>();

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            await _userRepository.DeleteAsync(user.UserId);
            await _userRepository.SaveAsync();

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

        public async Task<ServiceResponse<User>> UpdatePassword(Guid userId, string currentPassword, string newPassword)
        {
            var response = new ServiceResponse<User>();

            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
            {
                response.Success = false;
                response.Message = "Invalid data";
                return response;
            }
            Console.WriteLine(userId);
            var user = await _userRepository.GetByIdAsync(userId);
            Console.WriteLine(user.UserId);
            Console.WriteLine(user.FullName);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }
            Console.WriteLine(user.Password);
            var mapPassword = await PasswordHashedUtils.VerifyPassword(currentPassword, user.Password);
            if(!mapPassword)
            {
                response.Success = false;
                response.Message = "False current Password";
                return response;
            }

            var hashedPassword = await PasswordHashedUtils.HashPassword(newPassword);
            user.Password = hashedPassword;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveAsync();
            var accessToken = _jwtUtils.GenerateAccessToken(user);
            response.AccessToken = accessToken;
            response.Data = user;
            response.Message = "Password updated successfully";
            response.Success = true;

            return response;
        }
        public async Task<ServiceResponse<string>> UpdateEmail(Guid userId, string newEmail)
        {
            var response = new ServiceResponse<string>();

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            // Kiểm tra email đã tồn tại chưa
            var emailExists = await _userRepository.GetUserByEmailAsync(newEmail);
            if (emailExists != null)
            {
                response.Success = false;
                response.Message = "Email already in use";
                return response;
            }
            //kiem tra neu dang nhap bang OAuthGoogle
            var isGoogleLogin = await _userRepository.GetUserByGoogleIdAsync(user.GoogleId);
            if (isGoogleLogin != null)
            {
                response.Success = false;
                response.Message = "Cannot change email for Google login user";
                return response;
            }
            // Cập nhật email + yêu cầu xác minh lại
            user.Email = newEmail;
            user.IsEmailVerified = false;
            var resetToken = _jwtUtils.GenerateResetToken(user);
            user.VerificationToken = resetToken;
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(10);
            user.EmailVerifiedAt = null;
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveAsync();

            // Gửi mail xác minh
            var verificationLink = $"https://localhost:7170/Site/Account/verify-email?token={resetToken}";
            await _emailUtils.SendEmailAsync(newEmail, "Verify your new email", $"Click here to verify: {verificationLink}");

            response.Data = "Verification email sent to new address";
            response.Message = "Email updated. Please verify your new email.";
            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<string>> LockUser(Guid userId)
        {
            var response = new ServiceResponse<string>();

            var user = await _userRepository.GetByIdAsync(userId);
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

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveAsync();

            response.Data = "User locked successfully";
            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<string>> UnlockUser(Guid userId)
        {
            var response = new ServiceResponse<string>();

            var user = await _userRepository.GetByIdAsync(userId);
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

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveAsync();

            response.Data = "User unlocked successfully";
            response.Success = true;
            return response;
        }
        public async Task<ServiceResponse<string>> UpdateAvatar(Guid userId, IFormFile file)
        {
            var response = new ServiceResponse<string>();

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            if (file != null && file.Length > 0)
            {
                var uploadResult = await _cloudinaryUtils.UploadImageAsync(file);
                user.AvatarUrl = uploadResult.ToString();
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveAsync();
                response.Data = user.AvatarUrl;
                response.Message = "Avatar updated successfully";
                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.Message = "Invalid file";
            }
            // Ghi lại thông tin người dùng vào cookie
            var accessToken = _jwtUtils.GenerateAccessToken(user);
            response.AccessToken = accessToken;
            return response;
        }
        public async Task<ServiceResponse<string>> UpdateUsername(Guid userId, string newUsername)
        {
            var response = new ServiceResponse<string>();

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            user.FullName = newUsername;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveAsync();

            response.Data = "Username updated successfully";
            response.Success = true;
            return response;
        }
    }
}