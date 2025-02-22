using Microsoft.AspNetCore.Mvc;

namespace StackBook.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Books()
        {
            return View();
        }
    }
}
