//using Microsoft.AspNetCore.Mvc;
//using StackBook.Services.Interfaces;
//using StackBook.ViewModels;
//using StackBook.Services.Interfaces;
//using StackBook.ViewModels;

//namespace StackBook.Controllers
//{
//    [Area("Customer")]
//    public class PaymentController : Controller
//    {
//        private readonly IVnPayService _vnPayService;

//        public PaymentController(IVnPayService vnPayService)
//        {
//            _vnPayService = vnPayService;
//        }

//        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
//        {
//            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
//            return Redirect(url);
//        }

//        [HttpGet]
//        public IActionResult PaymentCallbackVnpay()
//        {
//            var response = _vnPayService.PaymentExecute(Request.Query);
//            return Json(response);
//        }
//    }
//}

using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using StackBook.Services;
using StackBook.Interfaces;


namespace StackBook.Controllers
{
    [Area("Customer")]
    public class PaymentController : ControllerBase
    {
        private readonly IVnpayService _vnpayService;


        public PaymentController(IVnpayService vnpayService)
        {
            _vnpayService = vnpayService;
        }

        [HttpPost]
        public IActionResult CreateOrder(int amount, string orderInfo)
        {
            string url = _vnpayService.CreateOrder(Request, amount, orderInfo, "https://localhost:7170/");
            return Redirect(url);
        }

        [HttpGet("return")]
        public IActionResult OrderReturn()
        {
            int result = _vnpayService.OrderReturn(Request);
            return Ok(new { status = result });
        }
    }
}


