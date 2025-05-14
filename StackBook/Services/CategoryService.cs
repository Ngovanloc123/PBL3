using Microsoft.EntityFrameworkCore;
using StackBook.DAL.IRepository;
using StackBook.Data;
using StackBook.Interfaces;
using StackBook.Models;
using StackBook.ViewModels;

namespace StackBook.Services
{
    public class CategoryService : ICategoryService
    {

        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


    }
}
