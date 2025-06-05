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
        private readonly IUnitOfWork _unitOfWork;

        public DiscountService(IDiscountRepository discountRepository, IUnitOfWork unitOfWork)
        {
            _discountRepository = discountRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Discount> CreateDefaultDiscount()
        {
            var discount = new Discount
            {
                DiscountId = Guid.NewGuid(),
                DiscountName = "No discount",
                Price = 0,
                Description = "No discount applies",
                DiscountCode = "0",
                CreatedDiscount = DateTime.Now,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddYears(1) // Hi?u l?c 1 n?m
            };
            await _unitOfWork.Discount.AddAsync(discount);
            await _unitOfWork.SaveAsync();
            return discount;
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

        public async Task<List<Discount>> GetActiveDiscounts(DateTime currentDate)
        {
            try
            {
                return await _discountRepository.GetActiveDiscountsAsync(currentDate);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve active discounts. Please try again later.", ex);
            }
        }
    }
}