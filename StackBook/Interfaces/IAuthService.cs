using StackBook.Models;
using System;
using StackBook.Services;
using StackBook.VMs;
using StackBook.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using static StackBook.ViewModels.UserVM;

namespace StackBook.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<User>> RegisterUser(RegisterVM registerVM);//dang ky thanh vien
        Task<ServiceResponse<User>> SignInUser(SignInVM signInVM);//dang nhap thanh vien
        Task<ServiceResponse<User>> LogoutUser(Guid userId);//dang xuat thanh vien
        Task<ServiceResponse<User>> ForgotPassword(ForgotPasswordVM forgotPasswordVM);//quen mat khau
        Task<ServiceResponse<User>> ResetPassword(ResetPasswordVM resetPasswordVM);//reset mat khau
        Task<ServiceResponse<User>> SendVerifyEmail(string email);//gui link kiem tra email
        Task<ServiceResponse<User>> ResendVerifyEmail(string email);//gui lai link kiem tra email
        Task<ServiceResponse<User>> VerifyEmail(string token);//kiem tra email
        Task<ServiceResponse<string>> RedirectGoogleConsentScreenAsync();//chuyen huong den man hinh consent google
        Task<ServiceResponse<User>> LoginWithGoogle(string code);//dang nhap bang google
    }
}