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
    public interface IAuthService
    {
        Task<ServiceResponse<User>> RegisterUser(RegisterDto registerDto);
        Task<ServiceResponse<User>> SignInUser(SignInDto signInDto);
        Task<ServiceResponse<User>> LogoutUser(Guid userId);
        Task<ServiceResponse<User>> ForgotPassword(ForgotPasswordDto forgotPasswordDto);
        Task<ServiceResponse<User>> ResetPassword(ResetPasswordDto resetPasswordDto);
        Task<ServiceResponse<User>> SendVerifyEmail(string email);
        Task<ServiceResponse<User>> ResendVerifyEmail(string email);
        Task<ServiceResponse<User>> VerifyEmail(string token);
        Task<ServiceResponse<string>> RedirectGoogleConsentScreenAsync();
        Task<ServiceResponse<User>> LoginWithGoogle(string code);
    }
}
