using StackBook.Interfaces;
using StackBook.Models;
using StackBook.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using StackBook.Exceptions;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using StackBook.DAL.IRepository;
using StackBook.DTOs;
using NuGet.Protocol;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.IdentityModel.Tokens;

namespace StackBook.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly ICartService _cartService;

        public OrderService(IOrderRepository orderRepository, IUserRepository userRepository, ICartService cartService, IDiscountRepository discountRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _cartService = cartService;
            _discountRepository = discountRepository;
        }
        public async Task CreateOrderAsync(Guid userId, string discountId, Guid shippingAddressId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null) throw new AppException($"Error: User with ID {userId}: Not Found");

                Guid? discountGuid = null;
                if (!string.IsNullOrWhiteSpace(discountId) && Guid.TryParse(discountId, out var parsedDiscountId))
                {
                    var discount = await _discountRepository.GetDiscountByIdAsync(parsedDiscountId);
                    if (discount != null)
                    {
                        discountGuid = parsedDiscountId;
                    }
                }

                double totalPrice = await _cartService.GetTotalPriceCartAsync(userId);

                var newOrder = new Order
                {
                    OrderId = Guid.NewGuid(),
                    UserId = userId,
                    //DiscountId = discountId,
                    ShippingAddressId = shippingAddressId,
                    TotalPrice = totalPrice
                };

                await _orderRepository.CreateOrderAsync(newOrder);
                await _orderRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new AppException($"Error: Creating order: {ex.Message}");
            }
        }


        public async Task UpdateOrderStatusAsync(int status, Guid orderId)
        {
            try
            {
                var order = await _orderRepository.FindOrderByIdAsync(orderId);
                if (order ==null) throw new AppException($"Error: Order with ID {orderId}: Not Found");

                await _orderRepository.UpdateOrderStatusAsync(orderId, status);
                await _orderRepository.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new AppException($"Error: Updating order's statu: {ex.Message}");
            }
        }
    
}