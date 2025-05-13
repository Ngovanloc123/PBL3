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
using System.Threading.Tasks;
using VNPAY.NET;

namespace StackBook.Services
{
    public class AuthService : IAuthService
    {
        private readonly EMailUtils _emailSender;  
        private readonly IUserRepository _userRepository;
        private readonly JwtUtils _jwtUtils;
        private readonly OAuthGoogleService _oauthGoogleService;

        public AuthService( EMailUtils emailSender, IUserRepository userRepository, JwtUtils jwtUtils, OAuthGoogleService oauthGoogleService)
        {
            _emailSender = emailSender;
            _userRepository = userRepository;
            _jwtUtils = jwtUtils;
            _oauthGoogleService = oauthGoogleService;
        }
        public async Task<ServiceResponse<User>> RegisterUser(RegisterDto registerDto)
        {
            var response = new ServiceResponse<User>();
            if(string.IsNullOrEmpty(registerDto.Email) || string.IsNullOrEmpty(registerDto.Password) || string.IsNullOrEmpty(registerDto.Username))
            {
                response.Success = false;
                response.Message = "Email, password and username are required";
                return response;
            }
            var existingUser = await _userRepository.GetUserByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                response.Success = false;
                response.Message = "Email already exists";
                return response;
            }
            var hashedPassword = await PasswordHashedUtils.HashPassword(registerDto.Password);
            var user = new User
            {
                Email = registerDto.Email,
                FullName = registerDto.Username,
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
            var verificationToken = _jwtUtils.GenerateToken(user);
            user.VerificationToken = verificationToken;
            var result = await _userRepository.CreateAsync(user);
            await _userRepository.SaveAsync();
            if (result != null)
            {
                var verificationLink = $"https://localhost:7170/Site/Account/verify-email?token={verificationToken}";
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
        public async Task<ServiceResponse<User>> SignInUser(SignInDto signInDto)
        {
            var response = new ServiceResponse<User>();
            if(string.IsNullOrEmpty(signInDto.Email) || string.IsNullOrEmpty(signInDto.Password))
            {
                response.Success = false;
                response.Message = "Email and password are required";
                Console.WriteLine($"SignIn result: {response}");
                return response;
            }
            var user = await _userRepository.GetUserByEmailAsync(signInDto.Email);
            Console.WriteLine($"User found: {user.Password}");
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
                Console.WriteLine($"SignIn result: {response.Message}");
                return response;
            }
            if(user.Password == null)
            {
                response.Success = false;
                response.Message = "Password is not set";
                Console.WriteLine($"SignIn result: {response.Message}");	
                return response;
            }
            bool matches = await PasswordHashedUtils.VerifyPassword(signInDto.Password, user.Password);
            Console.WriteLine($"Password verification result: {matches}");
            Console.WriteLine($"User password: {user.Password}");
            Console.WriteLine($"Input password: {signInDto.Password}");
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
            await _userRepository.SaveAsync();
            response.Success = true;
            response.Data = user;
            response.AccessToken = accessToken;
            response.RefreshToken = refreshToken;
            response.StatusCode = 200;
            response.Message = "Login successful";
            Console.WriteLine($"SignIn result: {response}");	
            return response;
        }
        public async Task<ServiceResponse<User>> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var response = new ServiceResponse<User>();
            if(string.IsNullOrEmpty(forgotPasswordDto.Email))
            {
                response.Success = false;
                response.Message = "Email is required";
                return response;
            }
            var user = await _userRepository.GetUserByEmailAsync(forgotPasswordDto.Email);
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
            await _userRepository.SaveAsync();
            var resetLink = $"https://localhost:7170/Site/Account/reset-password?token={resetToken}";
            var subject = "Reset Password";
            var message = $"Please reset your password by clicking this link: {resetLink}";
            await _emailSender.SendEmailAsync(user.Email, subject, message);
            response.Success = true; 
            response.Message = "Reset password link sent to email";
            response.Data = user;
            return response;
        }
        public async Task<ServiceResponse<User>> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var response = new ServiceResponse<User>();
            if(string.IsNullOrEmpty(resetPasswordDto.Token) || string.IsNullOrEmpty(resetPasswordDto.NewPassword) || string.IsNullOrEmpty(resetPasswordDto.ConfirmPassword))
            {
                response.Success = false;
                response.Message = "Token, new password and confirm password are required";
                return response;
            }
            if(resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
            {
                response.Success = false;
                response.Message = "Passwords do not match";
                return response;
            }
            var user = await _userRepository.GetUserByResetTokenAsync(resetPasswordDto.Token);
            if(user == null || user.ResetTokenExpiry < DateTime.UtcNow)
            {
                response.Success = false;
                response.Message = "Invalid or expired token";
                return response;
            }
            var hashedPassword = await PasswordHashedUtils.HashPassword(resetPasswordDto.NewPassword);
            user.Password = hashedPassword;
            user.ResetPasswordToken = null;
            user.ResetTokenExpiry = null;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveAsync();
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
            await _userRepository.SaveAsync();
            var verificationLink = $"https://localhost:7170/Site/Account/verify-email?token={verificationToken}";
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
            await _userRepository.SaveAsync();
            var verificationLink = $"https://localhost:7170/Site/Account/verify-email?token={verificationToken}";
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
            Console.WriteLine($"Token: {token}");
            var response = new ServiceResponse<User>();
            if(string.IsNullOrEmpty(token))
            {
                response.Success = false;
                response.Message = "Token is required";
                return response;
            }
            var user = await _userRepository.GetUserByVerificationTokenAsync(token);
            Console.WriteLine($"User found: {user.VerificationToken}");
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
            await _userRepository.SaveAsync();
            response.Success = true;
            response.Message = "Email verified successfully";
            response.Data = user;
            return response;
        }
        //Verify reset password token
        public async Task<ServiceResponse<User>> VerifyResetPasswordToken(string token)
        {
            var response = new ServiceResponse<User>();
            if(string.IsNullOrEmpty(token))
            {
                response.Success = false;
                response.Message = "Token is required";
                return response;
            }
            var user = await _userRepository.GetUserByResetTokenAsync(token);
            if(user == null || user.ResetTokenExpiry < DateTime.UtcNow)
            {
                response.Success = false;
                response.Message = "Invalid or expired token";
                return response;
            }
            response.Success = true;
            response.Message = "Token is valid";
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
            await _userRepository.SaveAsync();
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
                Console.WriteLine($"Redirect URL: {url}");
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
                Console.WriteLine($"Google ID: {googleId}");
                Console.WriteLine($"Email: {email}");
                Console.WriteLine($"Name: {name}");
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
                            Role = false,
                            EmailVerifiedAt = DateTime.UtcNow,
                            LockStatus = false,
                        };
                        await _userRepository.CreateGoogleUserAsync(user);
                    }
                    else
                    {
                        user.GoogleId = googleId;
                        await _userRepository.UpdateAsync(user);
                    }
                }
                Console.WriteLine($"User found: {user.Email}");
                Console.WriteLine($"User Google ID: {user.GoogleId}");
                var refreshToken = _jwtUtils.GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveAsync();
                response.Success = true;
                response.Data = user;
                response.Message = "Login with Google successful";
                response.AccessToken = _jwtUtils.GenerateAccessToken(user);
                response.RefreshToken = refreshToken;
                response.StatusCode = StatusCodes.Status200OK;
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