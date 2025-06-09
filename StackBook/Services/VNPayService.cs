//using Microsoft.AspNetCore.Http;
//using StackBook.Services.Interfaces;
//using StackBook.ViewModels;
//using StackBook.Services.Interfaces;
//using StackBook.ViewModels;

//namespace StackBook.Services
//{
//    public class VnPayService : IVnPayService
//    {
//        private readonly IConfiguration _configuration;

//        public VnPayService(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
//        {
//            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
//            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
//            var tick = DateTime.Now.Ticks.ToString();
//            var pay = new VnPayLibrary();
//            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];

//            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
//            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
//            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
//            pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
//            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
//            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
//            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
//            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
//            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
//            pay.AddRequestData("vnp_OrderType", model.OrderType);
//            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
//            pay.AddRequestData("vnp_TxnRef", tick);

//            var paymentUrl = pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
//            return paymentUrl;
//        }

//        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
//        {
//            var pay = new VnPayLibrary();
//            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);
//            return response;
//        }
//    }
//}

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using StackBook.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace StackBook.Services
{
    public class VnpayService: IVnpayService
    {
        public string CreateOrder(HttpRequest request, int amount, string orderInfo, string urlReturn)
        {
            string vnp_Version = "2.1.0";
            string vnp_Command = "pay";
            string vnp_TxnRef = VnpayConfig.GetRandomNumber(8);
            string vnp_IpAddr = "127.0.0.1";
            string vnp_TmnCode = VnpayConfig.vnp_TmnCode;
            string orderType = "order-type";

            var vnp_Params = new SortedDictionary<string, string>
            {
                { "vnp_Version", vnp_Version },
                { "vnp_Command", vnp_Command },
                { "vnp_TmnCode", vnp_TmnCode },
                { "vnp_Amount", (amount * 100).ToString() },
                { "vnp_CurrCode", "VND" },
                { "vnp_TxnRef", vnp_TxnRef },
                { "vnp_OrderInfo", orderInfo },
                { "vnp_OrderType", orderType },
                { "vnp_Locale", "vn" },
                { "vnp_ReturnUrl", urlReturn + VnpayConfig.vnp_Returnurl },
                { "vnp_IpAddr", vnp_IpAddr },
                { "vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss") },
                { "vnp_ExpireDate", DateTime.UtcNow.AddHours(7).AddMinutes(15).ToString("yyyyMMddHHmmss") }
            };

            string hashData = VnpayConfig.BuildDataToHash(vnp_Params);
            string queryString = VnpayConfig.BuildQueryString(vnp_Params);
            string vnp_SecureHash = VnpayConfig.HmacSHA512(VnpayConfig.vnp_HashSecret, hashData);

            return $"{VnpayConfig.vnp_PayUrl}?{queryString}&vnp_SecureHash={vnp_SecureHash}";
        }

        public int OrderReturn(HttpRequest request)
        {
            var fields = new SortedDictionary<string, string>();
            foreach (var key in request.Query.Keys)
            {
                if (key != "vnp_SecureHash" && key != "vnp_SecureHashType")
                {
                    fields[key] = request.Query[key];
                }
            }

            string vnp_SecureHash = request.Query["vnp_SecureHash"];
            string signValue = VnpayConfig.HashAllFields(fields);

            if (signValue.Equals(vnp_SecureHash, StringComparison.InvariantCultureIgnoreCase))
            {
                return request.Query["vnp_TransactionStatus"] == "00" ? 1 : 0;
            }
            return -1;
        }
    }
}
