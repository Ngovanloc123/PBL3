using Microsoft.AspNetCore.Mvc;

namespace StackBook.Controllers
{
    public class ManageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
