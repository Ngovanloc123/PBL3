using Microsoft.AspNetCore.Mvc;
using StackBook.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using StackBook.Data;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.VariantTypes;
using StackBook.ViewModels;
using StackBook.DAL.IRepository;
using StackBook.Interfaces;

namespace StackBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class RecommendController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReviewService _reviewService;

        public RecommendController(IHttpClientFactory httpClientFactory, IUnitOfWork unitOfWork, IReviewService reviewService)
        {
            _httpClientFactory = httpClientFactory;
            _unitOfWork = unitOfWork;
            _reviewService = reviewService;
        }

        [HttpPost]
        public async Task<IActionResult> Index(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                TempData["Error"] = "Bạn chưa nhập truy vấn.";
                return RedirectToAction("Index");
            }

            // Gửi truy vấn đến API Python
            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(new { query }), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://127.0.0.1:5000/recommend", content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Lỗi kết nối đến hệ thống gợi ý.";
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();

            // Deserialize thành List<string>
            var bookIdStrings = JsonSerializer.Deserialize<List<string>>(json);

            if (bookIdStrings == null || !bookIdStrings.Any())
            {
                TempData["Error"] = "Không có sách được gợi ý.";
                return RedirectToAction("Index");
            }

            // Chuyển chuỗi sang Guid
            List<Guid> bookIds;
            try
            {
                bookIds = bookIdStrings.Select(idStr => Guid.Parse(idStr)).ToList();
            }
            catch (FormatException)
            {
                TempData["Error"] = "Dữ liệu ID sách không đúng định dạng.";
                return RedirectToAction("Index");
            }

            Console.WriteLine("Recommended book IDs:");
            //foreach (var id in bookIds)
            //{
            //    Console.WriteLine($"- {id}");
            //}

            //// Lấy sách từ DB theo các ID gợi ý
            //var books = _context.Books
            //                    .Where(b => bookIds.Contains(b.BookId))
            //                    .ToList();


            var bookRating = new List<BookRatingViewModel>();
            foreach(var id in bookIds)
            {
                var book = await _unitOfWork.Book.GetAsync(b => b.BookId == id);
                var averageRating = await _reviewService.GetAverageRatingForBookAsync(id);
                bookRating.Add(new BookRatingViewModel
                {
                    Book = book,
                    AverageRating = averageRating
                });

            }

            // Lưu vào session
            HttpContext.Session.SetObject("RecommendedBooks", bookRating);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Index()
        {
            var books = HttpContext.Session.GetObject<List<BookRatingViewModel>>("RecommendedBooks") ?? new List<BookRatingViewModel>();
            return View(books);
        }
    }
}
