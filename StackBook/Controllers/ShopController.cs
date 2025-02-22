using Microsoft.AspNetCore.Mvc;

namespace StackBook.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult ProductDetail()
        {
            return View();
        }
    }
}
