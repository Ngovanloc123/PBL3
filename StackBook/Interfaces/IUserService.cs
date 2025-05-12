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
        Task<ServiceResponse<User>> UpdateUser(UpdateDto updateDto);//sua thong tin
        Task<ServiceResponse<User>> DeleteUser(Guid userId);//xoa nguoi dung
        Task<ServiceResponse<List<User>>> GetAllUsers();//in ra tat ca user
        Task<ServiceResponse<User>> GetUserByName(string username);//tim kiem user theo ten
        Task<ServiceResponse<User>> GetUserById(Guid userId);//tim kiem user theo id
        Task<ServiceResponse<User>> GetUserByEmail(string email);//tim kiem user theo email
        Task<ServiceResponse<User>> UpdatePassword(UpdatePasswordDto updatePasswordDto);//doi mat khau
        Task<ServiceResponse<string>> UpdateEmail(Guid userId, string newEmail);//doi email
        Task<ServiceResponse<string>> LockUser(Guid userId);//khoa nguoi dung
        Task<ServiceResponse<string>> UnlockUser(Guid userId);//mo khoa nguoi dung
        Task<ServiceResponse<string>> UpdateAvatar(Guid userId, IFormFile file);
    }
}
