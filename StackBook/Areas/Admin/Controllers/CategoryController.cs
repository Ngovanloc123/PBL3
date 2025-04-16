using Microsoft.AspNetCore.Mvc;
using StackBook.Data;
using StackBook.Services;
using StackBook.Models;
using StackBook.DAL.IRepository;
using Microsoft.EntityFrameworkCore;


namespace StackBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }
        // Dùng để gửi dữ liệu từ form, cập nhật database. Dữ liệu không hiển thị trên URL.
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(Guid? CategoryId)
        {
            if (CategoryId == null)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.CategoryId == CategoryId);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        // Dùng để gửi dữ liệu từ form, cập nhật database. Dữ liệu không hiển thị trên URL.
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category edited successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(Guid? CategoryId)
        {
            if (CategoryId == null)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.CategoryId == CategoryId);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        // Dùng để gửi dữ liệu từ form, cập nhật database. Dữ liệu không hiển thị trên URL.
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(Guid? CategoryId)
        {
            Category? obj = _unitOfWork.Category.Get(u => u.CategoryId == CategoryId);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
