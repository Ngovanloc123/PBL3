using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackBook.DAL.IRepository;
using StackBook.DAL.Repository;
using StackBook.Data;
using StackBook.Interfaces;
using StackBook.Models;
using StackBook.ViewModels;

namespace StackBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShippingAddressController : Controller
    {
        private readonly IShippingAddressService _shippingAddressService;
        private readonly IUnitOfWork _unitOfWork;

        public ShippingAddressController(IUnitOfWork unitOfWork, IShippingAddressService shippingAddressService)
        {
            _unitOfWork = unitOfWork;
            _shippingAddressService = shippingAddressService;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShippingAddress shippingAddress)
        {
            if (ModelState.IsValid)
            {
                await _shippingAddressService.Add(shippingAddress);
                await _unitOfWork.SaveAsync();
                ViewData["success"] = "Add shipping address successful";

                return RedirectToAction("Checkout", "Cart");
            }
            return RedirectToAction("Checkout", "Cart");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Select(Guid ShippingAddressId)
        {
            // Lấy dữ liệu từ session
            var sessionData = HttpContext.Session.GetString("CheckoutRequest");
            if (string.IsNullOrEmpty(sessionData))
            {
                TempData["Error"] = "No checkout data found.";
                return RedirectToAction("Index", "Cart");
            }

            // Parse session thành CheckoutRequest
            var checkoutRequest = JsonConvert.DeserializeObject<CheckoutRequest>(sessionData);

            // Lấy thông tin địa chỉ từ database theo ID
            var selectedAddress = await _unitOfWork.ShippingAddress.GetAsync(addr => addr.ShippingAddressId == ShippingAddressId);
            if (selectedAddress == null)
            {
                TempData["Error"] = "Shipping address not found.";
                return RedirectToAction("Checkout", "Cart");
            }
            if (checkoutRequest == null)
            {
                TempData["Error"] = "Checkout request not found.";
                return RedirectToAction("Index", "Cart");
            }
            // Cập nhật địa chỉ mặc định
            checkoutRequest.shippingAddressDefault = selectedAddress;

            // Ghi lại vào session
            HttpContext.Session.SetString("CheckoutRequest", JsonConvert.SerializeObject(checkoutRequest));

            TempData["success"] = "Change shipping address successfully!";

            // Quay lại trang Checkout
            return RedirectToAction("Checkout", "Cart");
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid ShippingAddressId)
        {
            var shippingAddress = await _unitOfWork.ShippingAddress.GetAsync(sa => sa.ShippingAddressId == ShippingAddressId);
            if (shippingAddress != null)
            {
                await _unitOfWork.ShippingAddress.DeleteAsync(shippingAddress);
                await _unitOfWork.SaveAsync();
            }

            var userId = Request.Cookies["userId"];
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index", "Cart");
            }

            // Cập nhật lại dữ liệu session
            var sessionData = HttpContext.Session.GetString("CheckoutRequest");
            if (string.IsNullOrEmpty(sessionData))
            {
                TempData["Error"] = "No checkout data found.";
                return RedirectToAction("Index", "Cart");
            }

            TempData["success"] = "Shipping address deleted successfully.";
            return RedirectToAction("Checkout", "Cart");
        }

    }
}