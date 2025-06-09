//using Microsoft.AspNetCore.Http;
//using StackBook.ViewModels;
//using StackBook.ViewModels;

//namespace StackBook.Services.Interfaces
//{
//    public interface IVnPayService
//    {
//        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
//        PaymentResponseModel PaymentExecute(IQueryCollection collections);
//    }
//}

using Microsoft.AspNetCore.Http;

namespace StackBook.Interfaces
{
    public interface IVnpayService
    {
        /// <summary>
        /// Tạo URL thanh toán VNPAY.
        /// </summary>
        /// <param name="request">HttpRequest hiện tại</param>
        /// <param name="amount">Số tiền thanh toán (đơn vị VND)</param>
        /// <param name="orderInfo">Thông tin đơn hàng</param>
        /// <param name="urlReturn">URL trả về sau khi thanh toán</param>
        /// <returns>URL để redirect tới trang thanh toán VNPAY</returns>
        string CreateOrder(HttpRequest request, int amount, string orderInfo, string urlReturn);

        /// <summary>
        /// Xử lý kết quả trả về từ VNPAY và kiểm tra tính hợp lệ của giao dịch.
        /// </summary>
        /// <param name="request">HttpRequest chứa các tham số trả về</param>
        /// <returns>1: thành công, 0: thất bại, -1: sai chữ ký</returns>
        int OrderReturn(HttpRequest request);
    }
}
