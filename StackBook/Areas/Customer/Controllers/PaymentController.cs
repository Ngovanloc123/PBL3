using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using StackBook.Interfaces;
using StackBook.ViewModels;
using System.Security.Claims;
using Newtonsoft.Json;
using Azure.Core;
using DocumentFormat.OpenXml.Office2016.Excel;
using StackBook.Models;

namespace MyMvcApp.Controllers
{
    [Area("Customer")]
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        public PaymentController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            Console.WriteLine($"Payment URL: {url}");
            return Redirect(url);
        }
        [HttpGet]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return NotFound("Invalid user.");
            }

            var sessionData = HttpContext.Session.GetString("CheckoutRequest");
            if (string.IsNullOrEmpty(sessionData))
            {
                TempData["Error"] = "No checkout data found.";
                return RedirectToAction("Index", "Cart");
            }
            var checkoutRequest = JsonConvert.DeserializeObject<CheckoutRequest>(sessionData);

            if (response.Success)
            {
                TempData["CheckoutRequest"] = JsonConvert.SerializeObject(checkoutRequest);
                return RedirectToAction("RedirectToPostPlaceOrder");
            }

            TempData["Error"] = "Payment failed.";
            return RedirectToAction("Index", "Cart");
        }

        public IActionResult RedirectToPostPlaceOrder()
        {
            if (TempData["CheckoutRequest"] is string checkoutRequestJson)
            {
                var checkoutRequest = JsonConvert.DeserializeObject<CheckoutRequest>(checkoutRequestJson);
                return View("RedirectToPost", checkoutRequest);
            }

            TempData["Error"] = "Checkout data missing.";
            return RedirectToAction("Index", "Cart");
        }
    }
}