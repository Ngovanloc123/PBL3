using StackBook.DAL.IRepository;
using StackBook.Interfaces;
using StackBook.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StackBook.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountService(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public async Task<Discount> GetDiscountByCode(string code)
        {
            try
            {
                return await _discountRepository.GetByCodeAsync(code);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to get discount with code {code}. Please try again later.", ex);
            }
        }

        public async Task<List<Discount>> GetAllDiscounts()
        {
            try
            {
                return await _discountRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve discounts. Please try again later.", ex);
            }
        }

        public async Task<Discount> CreateDiscount(Discount discount)
        {
            try
            {
                if (discount == null)
                {
                    throw new ArgumentNullException(nameof(discount), "Discount cannot be null");
                }

                await _discountRepository.AddAsync(discount);
                return discount;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to create discount. Please try again later.", ex);
            }
        }

        public async Task<Discount> UpdateDiscount(Discount discount)
        {
            try
            {
                if (discount == null)
                {
                    throw new ArgumentNullException(nameof(discount), "Discount cannot be null");
                }

                await _discountRepository.UpdateAsync(discount);
                return discount;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to update discount with ID {discount?.DiscountId}. Please try again later.", ex);
            }
        }

        public async Task<Discount> DeleteDiscount(Discount discount)
        {
            try
            {
                if (discount == null)
                {
                    throw new ArgumentNullException(nameof(discount), "Discount cannot be null");
                }

                await _discountRepository.DeleteAsync(discount.DiscountId);
                return discount;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to delete discount with ID {discount?.DiscountId}. Please try again later.", ex);
            }
        }
    }
}