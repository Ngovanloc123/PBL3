using StackBook.Models;
using System;
using StackBook.Services;
using StackBook.DTOs;
using StackBook.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using StackBook.ViewModels;

namespace StackBook.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<User>> RegisterUser(UserVM.RegisterVM registerDto);//dang ky thanh vien
        Task<ServiceResponse<User>> SignInUser(UserVM.SignInVM signInDto);//dang nhap thanh vien
        Task<ServiceResponse<User>> LogoutUser(Guid userId);//dang xuat thanh vien
        Task<ServiceResponse<User>> ForgotPassword(UserVM.ForgotPasswordVM forgotPasswordDto);//quen mat khau
        Task<ServiceResponse<User>> ResetPassword(UserVM.ResetPasswordVM resetPasswordDto);//reset mat khau
        Task<ServiceResponse<User>> SendVerifyEmail(string email);//gui link kiem tra email
        Task<ServiceResponse<User>> ResendVerifyEmail(string email);//gui lai link kiem tra email
        Task<ServiceResponse<User>> VerifyEmail(string token);//kiem tra email
        Task<ServiceResponse<string>> RedirectGoogleConsentScreenAsync();//chuyen huong den man hinh consent google
        Task<ServiceResponse<User>> LoginWithGoogle(string code);//dang nhap bang google
    }
}
