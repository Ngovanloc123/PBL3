
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Mvc;
using StackBook.Data;
using StackBook.Services;
using StackBook.Models;
using DocumentFormat.OpenXml.InkML;
using StackBook.DAL.IRepository;
using Microsoft.EntityFrameworkCore;
using StackBook.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

namespace StackBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookController : Controller
    {
        
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {

            _UnitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<BookDetailViewModel> BookDetail = _UnitOfWork.Book.GetAllBookDetails().ToList();
            
            return View(BookDetail);
        }

        public IActionResult Create()
        {
            // Lấy danh sách Category
            IEnumerable<SelectListItem> CategoryList = _UnitOfWork.Category
            .GetAll().Select(u => new SelectListItem
            {
                Text = u.CategoryName,
                Value = u.CategoryName
            });

            ViewBag.CategoryList = CategoryList;

            // Lấy danh sách Author
            IEnumerable<SelectListItem> AuthorList = _UnitOfWork.Author
                .GetAll().Select(a => new SelectListItem
                {
                    Text = a.AuthorName,
                    Value = a.AuthorName
                });

            ViewBag.AuthorList = AuthorList;

            return View();
        }

        [HttpPost]
        public IActionResult Create(BookDetailViewModel bookCreate, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                // Lấy đường dẫn file tĩnh wwwroot
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); // Lấy đuôi hình ảnh
                    string bookPath = Path.Combine(wwwRootPath, @"images/Books");

                    if (!string.IsNullOrEmpty(bookCreate.ImageURL))
                    {
                        // Xóa hình ảnh trước đó
                        var oldImagePath = Path.Combine(wwwRootPath, bookCreate.ImageURL.TrimStart('/'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(bookPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    bookCreate.ImageURL = @"/images/Books/" + fileName;

                }    

                _UnitOfWork.Book.AddBookDetail(bookCreate);
                _UnitOfWork.Save();
                TempData["success"] = "Book created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(Guid? BookId)
        {
            if (BookId == null)
            {
                return NotFound();
            }
            BookDetailViewModel? BookDetail = _UnitOfWork.Book.GetBookDetail(BookId);
            if (BookDetail == null)
            {
                return NotFound();
            }
            // Lấy danh sách Category
            IEnumerable<SelectListItem> CategoryList = _UnitOfWork.Category
            .GetAll().Select(u => new SelectListItem
            {
                Text = u.CategoryName,
                Value = u.CategoryName.ToString()
            });

            ViewBag.CategoryList = CategoryList;

            // Lấy danh sách Author
            IEnumerable<SelectListItem> AuthorList = _UnitOfWork.Author
                .GetAll().Select(a => new SelectListItem
                {
                    Text = a.AuthorName,
                    Value = a.AuthorName
                });

            ViewBag.AuthorList = AuthorList;

            return View(BookDetail);
        }

        [HttpPost]
        public IActionResult Edit(BookDetailViewModel bookEdit, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                // Lấy đường dẫn file tĩnh wwwroot
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); // Lấy đuôi hình ảnh
                    string bookPath = Path.Combine(wwwRootPath, @"images/Books");

                    if (!string.IsNullOrEmpty(bookEdit.ImageURL))
                    {
                        // Xóa hình ảnh trước đó
                        var oldImagePath = Path.Combine(wwwRootPath, bookEdit.ImageURL.TrimStart('/'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(bookPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    bookEdit.ImageURL = @"/images/Books/" + fileName;

                }
                _UnitOfWork.Book.UpdateBookDetail(bookEdit);
                _UnitOfWork.Save();

                TempData["success"] = "Book edited successfully";

                return RedirectToAction("Index");
            }

            return View();
        }





        //public IActionResult Delete(Guid? BookId)
        //{
        //    if (BookId == null)
        //    {
        //        return NotFound();
        //    }
        //    BookDetailViewModel? bookDelete = _UnitOfWork.Book.GetBookDetail(BookId);
        //    if (bookDelete == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(bookDelete);
        //}

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(Guid? BookId)
        {
            BookDetailViewModel? bookDelete = _UnitOfWork.Book.GetBookDetail(BookId);
            if (bookDelete == null)
            {
                return NotFound();
            }
            _UnitOfWork.Book.DeleteBookDetail(bookDelete);
            _UnitOfWork.Save();
            TempData["success"] = "Book deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
