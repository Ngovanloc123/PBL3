using Microsoft.AspNetCore.Mvc;
using StackBook.DAL.IRepository;
using StackBook.ViewModels;

namespace StackBook.Areas.Customer.Controllers
{
    public class CartController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork; // Correct type for _UnitOfWork  

        public CartController(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }

        [Area("Customer")]
        public IActionResult Index()
        {
            var books = _UnitOfWork.Book.GetAll();
            return View(books); 
        }
    }
}
