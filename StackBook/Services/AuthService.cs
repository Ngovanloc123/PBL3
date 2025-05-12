using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using StackBook.Data;
using StackBook.VMs;
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
using System.Threading.Tasks;
using VNPAY.NET;
using static StackBook.ViewModels.UserVM;

namespace StackBook.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly EMailUtils _emailSender;  
        private readonly IUserRepository _userRepository;
        private readonly JwtUtils _jwtUtils;
        private readonly OAuthGoogleService _oauthGoogleService;

        private string avatarDefault = "https://res.cloudinary.com/dzkbuah8k/image/upload/v1746291744/avatar_default_y8oo4n.jpg\r\n";

        public AuthService( EMailUtils emailSender, IUserRepository userRepository, JwtUtils jwtUtils, OAuthGoogleService oauthGoogleService, IUnitOfWork unitOfWork)
        {
            _emailSender = emailSender;
            _userRepository = userRepository;
            _jwtUtils = jwtUtils;
            _oauthGoogleService = oauthGoogleService;
            _unitOfWork = unitOfWork;
        }
        public async Task<ServiceResponse<User>> RegisterUser(RegisterVM registerVM)
        {
            var response = new ServiceResponse<User>();
            if(string.IsNullOrEmpty(registerVM.Email) || string.IsNullOrEmpty(registerVM.Password) || string.IsNullOrEmpty(registerVM.Username))
            {
                response.Success = false;
                response.Message = "Email, password and username are required";
                return response;
            }
            var existingUser = await _userRepository.GetUserByEmailAsync(registerVM.Email);
            if (existingUser != null)
            {
                response.Success = false;
                response.Message = "Email already exists";
                return response;
            }
            var hashedPassword = await PasswordHashedUtils.HashPassword(registerVM.Password);
            var user = new User
            {
                Email = registerVM.Email,
                FullName = registerVM.Username,
                AvatarURL = avatarDefault,
                Password = hashedPassword,
                Role = false,
                CreatedUser = DateTime.UtcNow,
                IsEmailVerified = false,
                VerificationToken = null,
                EmailVerifiedAt = null,
                ResetPasswordToken = null,
                ResetTokenExpiry = null,
                LockStatus = false,
                DateLock = null,
                AmountOfTime = 0,
                RefreshToken = null,
                RefreshTokenExpiry = null,
            };
            var result = await _userRepository.CreateAsync(user);
            await _unitOfWork.SaveAsync();
            if (result != null)
            {
                var verificationToken = _jwtUtils.GenerateToken(user);
                user.VerificationToken = verificationToken;
                var verificationLink = $"https://localhost:7170/auth/verify-email?token={verificationToken}";
                var subject = "Email Verification";
                var message = $"Please verify your email by clicking this link: {verificationLink}";
                await _emailSender.SendEmailAsync(user.Email, subject, message);
                response.Data = user;
                response.Success = true;
                response.Message = "User registered successfully";
                return response;
            }
            else
            {
                response.Success = false;
                response.Message = "User registration failed";
                return response;
            }
        }
        public async Task<ServiceResponse<User>> SignInUser(SignInVM signInVM)
        {
            var response = new ServiceResponse<User>();
            if(string.IsNullOrEmpty(signInVM.Email) || string.IsNullOrEmpty(signInVM.Password))
            {
                response.Success = false;
                response.Message = "Email and password are required";
                Console.WriteLine($"SignIn result: {response}");
                return response;
            }
            var user = await _userRepository.GetUserByEmailAsync(signInVM.Email);
            //Console.WriteLine($"User found: {user.Password}");
            if(user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                Console.WriteLine($"SignIn result: {response.Message}");
                return response;
            }
            if(user.LockStatus == true)
            {
                response.Success = false;
                response.Message = "Account is locked";
                Console.WriteLine($"SignIn result: {response.Message}");
                return response;
            }
            if(user.IsEmailVerified == false)
            {
                response.Success = false;
                response.Message = "Email is not verified";
                //Console.WriteLine($"SignIn result: {response.Message}");
                return response;
            }
            if(user.Password == null)
            {
                response.Success = false;
                response.Message = "Password is not set";
                //Console.WriteLine($"SignIn result: {response.Message}");	
                return response;
            }
            bool matches = await PasswordHashedUtils.VerifyPassword(signInVM.Password, user.Password);
            //Console.WriteLine($"Password verification result: {matches}");
            //Console.WriteLine($"User password: {user.Password}");
            //Console.WriteLine($"Input password: {signInVM.Password}");
            if(!matches)
            {
                response.Success = false;
                response.Message = "Invalid password";
                Console.WriteLine($"SignIn result: {response.Message}");	
                return response;
            }
            var accessToken = _jwtUtils.GenerateAccessToken(user);
            var refreshToken = _jwtUtils.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();
            response.Success = true;
            response.Data = user;
            response.AccessToken = accessToken;
            response.RefreshToken = refreshToken;
            response.StatusCode = 200;
            response.Message = "Login successful";
            //Console.WriteLine($"SignIn result: {response}");	
            return response;
        }
        public async Task<ServiceResponse<User>> ForgotPassword(ForgotPasswordVM forgotPasswordVM)
        {
            var response = new ServiceResponse<User>();
            if(string.IsNullOrEmpty(forgotPasswordVM.Email))
            {
                response.Success = false;
                response.Message = "Email is required";
                return response;
            }
            var user = await _userRepository.GetUserByEmailAsync(forgotPasswordVM.Email);
            if(user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }
            var resetToken = _jwtUtils.GenerateResetToken(user);
            user.ResetPasswordToken = resetToken;
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();
            var resetLink = $"https://localhost:7170/auth/reset-password?token={resetToken}";
            var subject = "Reset Password";
            var message = $"Please reset your password by clicking this link: {resetLink}";
            await _emailSender.SendEmailAsync(user.Email, subject, message);
            response.Success = true; 
            response.Message = "Reset password link sent to email";
            response.Data = user;
            return response;
        }
        public async Task<ServiceResponse<User>> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            var response = new ServiceResponse<User>();
            if(string.IsNullOrEmpty(resetPasswordVM.Token) || string.IsNullOrEmpty(resetPasswordVM.NewPassword) || string.IsNullOrEmpty(resetPasswordVM.ConfirmPassword))
            {
                response.Success = false;
                response.Message = "Token, new password and confirm password are required";
                return response;
            }
            if(resetPasswordVM.NewPassword != resetPasswordVM.ConfirmPassword)
            {
                response.Success = false;
                response.Message = "Passwords do not match";
                return response;
            }
            var user = await _userRepository.GetUserByResetTokenAsync(resetPasswordVM.Token);
            if(user == null || user.ResetTokenExpiry < DateTime.UtcNow)
            {
                response.Success = false;
                response.Message = "Invalid or expired token";
                return response;
            }
            var hashedPassword = await PasswordHashedUtils.HashPassword(resetPasswordVM.NewPassword);
            user.Password = hashedPassword;
            user.ResetPasswordToken = null;
            user.ResetTokenExpiry = null;
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();
            response.Success = true;
            response.Message = "Password reset successfully";
            response.Data = user;
            return response;
        }
        public async Task<ServiceResponse<User>> SendVerifyEmail(string email)
        {
            var response = new ServiceResponse<User>();
            if(string.IsNullOrEmpty(email))
            {
                response.Success = false;
                response.Message = "Email is required";
                return response;
            }
            var user = await _userRepository.GetUserByEmailAsync(email);
            if(user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }
            if(user.IsEmailVerified == true)
            {
                response.Success = false;
                response.Message = "Email is already verified";
                return response;
            }
            var verificationToken = _jwtUtils.GenerateResetToken(user);
            user.VerificationToken = verificationToken;
            user.EmailVerifiedAt = DateTime.UtcNow.AddMinutes(15);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();
            var verificationLink = $"https://localhost:7170/auth/verify-email?token={verificationToken}";
            var subject = "Email Verification";
            var message = $"Please verify your email by clicking this link: {verificationLink}";
            await _emailSender.SendEmailAsync(user.Email, subject, message);
            response.Success = true;
            response.Message = "Verification email sent successfully";
            response.Data = user;
            return response;
        }
        public async Task<ServiceResponse<User>> ResendVerifyEmail(string email)
        {
            var response = new ServiceResponse<User>();
            if(string.IsNullOrEmpty(email))
            {
                response.Success = false;
                response.Message = "Email is required";
                return response;
            }
            var user = await _userRepository.GetUserByEmailAsync(email);
            if(user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }
            if(user.IsEmailVerified == true)
            {
                response.Success = false;
                response.Message = "Email is already verified";
                return response;
            }
            var verificationToken = _jwtUtils.GenerateResetToken(user);
            user.VerificationToken = verificationToken;
            user.EmailVerifiedAt = DateTime.UtcNow.AddMinutes(15);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();
            var verificationLink = $"https://localhost:7170/auth/verify-email?token={verificationToken}";
            var subject = "Email Verification";
            var message = $"Please verify your email by clicking this link: {verificationLink}";
            await _emailSender.SendEmailAsync(user.Email, subject, message);
            response.Success = true;
            response.Message = "Verification email resent successfully";
            response.Data = user;
            return response;
        }
        public async Task<ServiceResponse<User>> VerifyEmail(string token)
        {
            var response = new ServiceResponse<User>();
            if(string.IsNullOrEmpty(token))
            {
                response.Success = false;
                response.Message = "Token is required";
                return response;
            }
            var user = await _userRepository.GetUserByResetTokenAsync(token);
            if(user == null || user.EmailVerifiedAt < DateTime.UtcNow)
            {
                response.Success = false;
                response.Message = "Invalid or expired token";
                return response;
            }
            user.IsEmailVerified = true;
            user.VerificationToken = null;
            user.EmailVerifiedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();
            response.Success = true;
            response.Message = "Email verified successfully";
            response.Data = user;
            return response;
        }
        public async Task<ServiceResponse<User>> GetUserByRefreshTokenAsync(string refreshToken)
        {
            var response = new ServiceResponse<User>();
            if(string.IsNullOrEmpty(refreshToken))
            {
                response.Success = false;
                response.Message = "Refresh token is required";
                return response;
            }
            var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);
            if(user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }
            response.Success = true;
            response.Data = user;
            response.RefreshToken = refreshToken;
            return response;
        }
        public async Task<ServiceResponse<User>> LogoutUser(Guid userId)
        {
            var response = new ServiceResponse<User>();
            if(string.IsNullOrEmpty(userId.ToString()))
            {
                response.Success = false;
                response.Message = "User ID is required";
                return response;
            }
            var user = await _userRepository.GetByIdAsync(userId);
            if(user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();
            response.Success = true;
            response.Message = "Logout successful";
            return response;
        }
        public async Task<ServiceResponse<string>> RedirectGoogleConsentScreenAsync()
        {
            try
            {
                var response = new ServiceResponse<string>();
                var url = await _oauthGoogleService.GetRedirectConsentScreenURL();
                if (string.IsNullOrEmpty(url))
                {
                    response.Success = false;
                    response.Message = "Failed to get redirect URL";
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Data = null;
                    return response;
                }
                response.Success = true;
                response.Message = "Login Sucessfully";
                response.Data = url;
                response.StatusCode = StatusCodes.Status200OK;
                return response;
            }
            catch
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "Error occurred while redirecting to Google consent screen",
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Data = null
                };
            }
        }
        public async Task<ServiceResponse<User>> LoginWithGoogle(string code)
        {
            var response = new ServiceResponse<User>();

            try
            {
                var accessToken = await _oauthGoogleService.GetAccessTokenAsync(code);
                var (email, name, googleId) = await _oauthGoogleService.GetGoogleUserProfileAsync(accessToken);

                var user = await _userRepository.GetUserByGoogleIdAsync(googleId);
                if (user == null)
                {
                    user = await _userRepository.GetUserByEmailAsync(email);
                    if (user == null)
                    {
                        user = new User
                        {
                            UserId = Guid.NewGuid(),
                            Email = email,
                            FullName = name,
                            GoogleId = googleId,
                            CreatedUser = DateTime.UtcNow,
                            IsEmailVerified = true,
                            Role = false
                        };
                        await _userRepository.CreateGoogleUserAsync(user);
                    }
                    else
                    {
                        user.GoogleId = googleId;
                        await _userRepository.UpdateAsync(user);
                    }
                }

                response.Success = true;
                response.Data = user;
                response.Message = "Login with Google successful";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error during Google login: {ex.Message}";
            }
            return response;
        }
    }
}