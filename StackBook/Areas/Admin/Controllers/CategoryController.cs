using Microsoft.AspNetCore.Mvc;
using StackBook.Data;
using StackBook.Services;
using StackBook.Models;
using StackBook.DAL.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace StackBook.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> categories = await _unitOfWork.Category.GetAllAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Category.AddAsync(obj);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> Edit(Guid? CategoryId)
        {
            if (CategoryId == null)
            {
                return NotFound();
            }
            Category? categoryFromDb = await _unitOfWork.Category.GetAsync(u => u.CategoryId == CategoryId);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Category.UpdateAsync(obj);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePOST(Guid? CategoryId)
        {
            if (CategoryId == null)
            {
                return NotFound();
            }
            Category? obj = await _unitOfWork.Category.GetAsync(u => u.CategoryId == CategoryId);
            if (obj == null)
            {
                return NotFound();
            }
            await _unitOfWork.Category.DeleteAsync(obj);
            await _unitOfWork.SaveAsync();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}