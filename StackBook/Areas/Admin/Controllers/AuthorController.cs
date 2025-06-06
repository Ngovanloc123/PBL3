using Microsoft.AspNetCore.Mvc;
using StackBook.Data;
using StackBook.Services;
using StackBook.Models;
using StackBook.DAL.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Extensions;

namespace StackBook.Areas.Admin.Controllers
{
    [Authorize]
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class AuthorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Admin/Author
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 8;
            int pageNumber = page ?? 1;

            var authors = await _unitOfWork.Author.GetAllAsync("Books");

            var pagedAuthors = authors.ToPagedList(pageNumber, pageSize);
            return View(pagedAuthors);
        }

        // GET: Admin/Author/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Author/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Author obj)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Author.AddAsync(obj);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "Author created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // GET: Admin/Author/Edit/5
        public async Task<IActionResult> Edit(Guid? AuthorId)
        {
            if (AuthorId == null)
            {
                return NotFound();
            }
            Author? authorFromDb = await _unitOfWork.Author.GetAsync(u => u.AuthorId == AuthorId);
            if (authorFromDb == null)
            {
                return NotFound();
            }
            return View(authorFromDb);
        }

        // POST: Admin/Author/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Author obj)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Author.UpdateAsync(obj);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "Author updated successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // POST: Admin/Author/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(Guid? AuthorId)
        {
            if (AuthorId == null)
            {
                return NotFound();
            }
            Author? obj = await _unitOfWork.Author.GetAsync(u => u.AuthorId == AuthorId);
            if (obj == null)
            {
                return NotFound();
            }
            await _unitOfWork.Author.DeleteAsync(obj);
            await _unitOfWork.SaveAsync();
            TempData["success"] = "Author deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
