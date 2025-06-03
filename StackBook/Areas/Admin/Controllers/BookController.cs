using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StackBook.DAL.IRepository;
using StackBook.Models;
using StackBook.ViewModels;
using X.PagedList;
using X.PagedList.Extensions;
using StackBook.Interfaces;

namespace StackBook.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class BookController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly INotificationService _notificationService;

        public BookController(IUnitOfWork unitOfWork, INotificationService notificationService, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _webHostEnvironment = webHostEnvironment;
        }

        // [GET] Admin/Book
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var books = await _unitOfWork.Book.GetAllAsync("Authors,Categories");

            var pagedBooks = books.ToPagedList(pageNumber, pageSize);


            return View(pagedBooks);
        }

        // [GET] Admin/Book/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = await BuildBookVMAsync();
            return View(viewModel);
        }

        // [POST] Admin/Book/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookVM viewModel, IFormFile? file)
        {
            // Nếu dữ liệu không hợp lệ
            if (!ModelState.IsValid)
            {
                // Tải lại các danh sách thể loại và tác giả
                viewModel.Categories = (await _unitOfWork.Category.GetAllAsync())
                    .Select(c => new SelectListItem { Text = c.CategoryName, Value = c.CategoryId.ToString() });
                viewModel.Authors = (await _unitOfWork.Author.GetAllAsync())
                    .Select(a => new SelectListItem { Text = a.AuthorName, Value = a.AuthorId.ToString() });
                //Gửi thông báo đến tất cả người dùng là có sách mới
                var allUsers = await _unitOfWork.User.GetAllAsync();
                //Check quyền nếu là user thì mới gửi
                foreach (var user in allUsers)
                {
                    if (user.Role == false)
                    {
                        await _notificationService.SendNotificationAsync(user.UserId, "New book added: " + viewModel.BookTitle);
                    }
                }
                return View(viewModel);
            }
            // ok
            var book = new Book
            {
                BookTitle = viewModel.BookTitle,
                Description = viewModel.Description,
                Price = viewModel.Price,
                Stock = viewModel.Stock,
                CreatedBook = DateTime.Now,
                Categories = (await _unitOfWork.Category.GetAllAsync())
                    .Where(c => viewModel.SelectedCategoryIds.Contains(c.CategoryId))
                    .ToList(),
                Authors = (await _unitOfWork.Author.GetAllAsync())
                    .Where(a => viewModel.SelectedAuthorIds.Contains(a.AuthorId))
                    .ToList()
            };

            if (file != null)
            {
                book.ImageURL = await SaveFileAsync(file);
            }

            await _unitOfWork.Book.AddAsync(book);
            await _unitOfWork.SaveAsync();
            TempData["success"] = "Book created successfully.";
            return RedirectToAction("Index");
        }

        // [GET] Admin/Book/Edit/{id}
        public async Task<IActionResult> Edit(Guid? bookId)
        {
            if (bookId == null)
            {
                return NotFound();
            }

            var book = await _unitOfWork.Book.GetAsync(b => b.BookId == bookId, "Authors,Categories");
            if (book == null)
            {
                return NotFound();
            }

            var viewModel = await BuildBookVMAsync(book);
            return View(viewModel);
        }

        // [POST] Admin/Book/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookVM viewModel, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                // Tải lại danh sách Categories và Authors
                viewModel.Categories = (await _unitOfWork.Category.GetAllAsync())
                    .Select(c => new SelectListItem { Text = c.CategoryName, Value = c.CategoryId.ToString() });
                viewModel.Authors = (await _unitOfWork.Author.GetAllAsync())
                    .Select(a => new SelectListItem { Text = a.AuthorName, Value = a.AuthorId.ToString() });

                return View(viewModel);
            }

            var book = await _unitOfWork.Book.GetAsync(b => b.BookId == viewModel.BookId, "Authors,Categories");
            if (book == null)
            {
                return NotFound();
            }

            book.BookTitle = viewModel.BookTitle;
            book.Description = viewModel.Description;
            book.Price = viewModel.Price;
            book.Stock = viewModel.Stock;

            if (file != null)
            {
                if (!string.IsNullOrEmpty(book.ImageURL))
                {
                    DeleteFile(book.ImageURL);
                }
                book.ImageURL = await SaveFileAsync(file);
            }

            book.Categories = (await _unitOfWork.Category.GetAllAsync())
                .Where(c => viewModel.SelectedCategoryIds.Contains(c.CategoryId))
                .ToList();
            book.Authors = (await _unitOfWork.Author.GetAllAsync())
                .Where(a => viewModel.SelectedAuthorIds.Contains(a.AuthorId))
                .ToList();

            await _unitOfWork.Book.UpdateAsync(book);
            await _unitOfWork.SaveAsync();
            TempData["success"] = "Book edited successfully.";
             //Gửi thông báo đến tất cả người dùng là có cập nhật
                var allUsers = await _unitOfWork.User.GetAllAsync();
                //Check quyền nếu là user thì mới gửi
                foreach (var user in allUsers)
                {
                    if (user.Role == false)
                    {
                        await _notificationService.SendNotificationAsync(user.UserId, "New book added: " + viewModel.BookTitle);
                    }
                }
            return RedirectToAction("Index");
        }

        // [POST] Admin/Book/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(Guid? bookId)
        {
            if (bookId == null)
            {
                return NotFound();
            }

            var book = await _unitOfWork.Book.GetAsync(b => b.BookId == bookId);
            if (book == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(book.ImageURL))
            {
                DeleteFile(book.ImageURL);
            }

            await _unitOfWork.Book.DeleteAsync(book);
            await _unitOfWork.SaveAsync();
            TempData["success"] = "Book deleted successfully.";
            return RedirectToAction("Index");
        }

        // Helper method to build BookViewModel
        private async Task<BookVM> BuildBookVMAsync(Book? book = null)
        {
            return new BookVM
            {
                BookId = book?.BookId,
                BookTitle = book?.BookTitle,
                Description = book?.Description,
                Price = book?.Price ?? 0,
                Stock = book?.Stock ?? 0,
                ImageURL = book?.ImageURL,
                SelectedCategoryIds = book?.Categories.Select(c => c.CategoryId).ToList() ?? new List<Guid>(),
                SelectedAuthorIds = book?.Authors.Select(a => a.AuthorId).ToList() ?? new List<Guid>(),
                // ViewBag
                Categories = (await _unitOfWork.Category.GetAllAsync())
                    .Select(c => new SelectListItem { Text = c.CategoryName, Value = c.CategoryId.ToString() }),
                Authors = (await _unitOfWork.Author.GetAllAsync())
                    .Select(a => new SelectListItem { Text = a.AuthorName, Value = a.AuthorId.ToString() })
            };
        }

        // Helper method to save file
        private async Task<string> SaveFileAsync(IFormFile file)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/Books", fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/images/Books/{fileName}";
        }

        // Helper method to delete file
        private void DeleteFile(string filePath)
        {
            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}