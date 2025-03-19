
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Mvc;
using StackBook.Data;
using StackBook.Services;
using StackBook.Models;
using DocumentFormat.OpenXml.InkML;
using StackBook.DAL.IRepository;
using Microsoft.EntityFrameworkCore;

namespace StackBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookController : Controller
    {
        
        private readonly IUnitOfWork _UnitOfWork;

        public BookController(IUnitOfWork unitOfWork)
        {

            _UnitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Book> categories = _UnitOfWork.Book.GetAll().ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }
        // Dùng để gửi dữ liệu từ form, cập nhật database. Dữ liệu không hiển thị trên URL.
        [HttpPost]
        public IActionResult Create(Book obj)
        {
            if(ModelState.IsValid)
            {
                _UnitOfWork.Book.Add(obj);
                _UnitOfWork.Save();
                TempData["success"] = "Book created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(Guid? BookId)
        {
            if(BookId == null)
            {
                return NotFound();
            }
            Book? bookFromDb = _UnitOfWork.Book.Get(u => u.BookId == BookId);
            if(bookFromDb == null)
            {
                return NotFound();
            }
            return View(bookFromDb);
        }

        // Dùng để gửi dữ liệu từ form, cập nhật database. Dữ liệu không hiển thị trên URL.
        [HttpPost]
        public IActionResult Edit(Book obj)
        {
            if (ModelState.IsValid)
            {
                _UnitOfWork.Book.Update(obj);
                _UnitOfWork.Save();
                TempData["success"] = "Book edited successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(Guid? BookId)
        {
            if (BookId == null)
            {
                return NotFound();
            }
            Book? bookFromDb = _UnitOfWork.Book.Get(u => u.BookId == BookId);
            if (bookFromDb == null)
            {
                return NotFound();
            }
            return View(bookFromDb);
        }

        // Dùng để gửi dữ liệu từ form, cập nhật database. Dữ liệu không hiển thị trên URL.
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(Guid? BookId)
        {
            Book? obj = _UnitOfWork.Book.Get(u => u.BookId == BookId);
            if (obj == null)
            {
                return NotFound();
            }
            _UnitOfWork.Book.Remove(obj);
            _UnitOfWork.Save();
            TempData["success"] = "Book deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
