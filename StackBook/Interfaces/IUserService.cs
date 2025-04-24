using StackBook.Models;
using System;
using StackBook.Services;
using StackBook.DTOs;
using StackBook.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace StackBook.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse<User>> RegisterUser(RegisterDto registerDto);
        Task<ServiceResponse<User>> LoginUser(LoginDto loginDto);
        Task<ServiceResponse<string>> RedirectGoogleConsentScreenAsync();
        Task<ServiceResponse<User>> LoginWithGoogle(string code);
        Task<ServiceResponse<User>> UpdateUser(UpdateDto updateDto);
        Task<ServiceResponse<User>> DeleteUser(Guid userId);
        Task<ServiceResponse<List<User>>> GetAllUsers();
        Task<ServiceResponse<User>> GetUserByName(string username);
        Task<ServiceResponse<User>> GetUserById(Guid userId);
        Task<ServiceResponse<User>> GetUserByEmail(string email);
        Task<ServiceResponse<User>> UpdatePassword(UpdatePasswordDto updatePasswordDto);
        Task<ServiceResponse<string>> UpdateEmail(Guid userId, string newEmail);
        Task<ServiceResponse<string>> ForgotPassword(ForgotPasswordDto forgotPasswordDto);
        Task<ServiceResponse<string>> ResetPassword(ResetPasswordDto resetPasswordDto);
        Task<ServiceResponse<string>> LockUser(Guid userId);
        Task<ServiceResponse<string>> UnlockUser(Guid userId);
        Task<ServiceResponse<string>> VerifyUser(string email);
        Task<ServiceResponse<string>> ResendVerificationEmail(string email);
        Task<ServiceResponse<string>> VerifyEmail(string token);
    }
}
