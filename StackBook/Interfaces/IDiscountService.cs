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
    public interface IDiscountService
    {
        Task<Discount> GetDiscountByCode(string code);
        Task<List<Discount>> GetAllDiscounts();
        Task<Discount> CreateDiscount(Discount discount);
        Task<Discount> UpdateDiscount(Discount discount);
        Task<Discount> DeleteDiscount(Discount discount);
    }
}