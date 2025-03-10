using Microsoft.AspNetCore.Mvc;

namespace StackBook.ViewComponents
{
    public class SliderCategoryViewComponent : ViewComponent
    {
        // Có thể lấy dữ liệu từ database
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
