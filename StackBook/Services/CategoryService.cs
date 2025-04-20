using Microsoft.EntityFrameworkCore;
using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.Models;
using StackBook.ViewModels;

namespace StackBook.Services
{
    public class CategoryService
    {

        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public string GetNameById(Guid? categoryId)
        {
            var category = _unitOfWork.Category.Get(c => c.CategoryId == categoryId);
            return category != null ? category.CategoryName : null;
        }


    }
}
