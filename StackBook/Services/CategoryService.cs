using Microsoft.EntityFrameworkCore;
using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.Models;
using StackBook.ViewModels;
using StackBook.Interfaces;

namespace StackBook.Services
{
    public class CategoryService: ICategoryService
    {

        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


    }
}
