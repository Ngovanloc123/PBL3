using Microsoft.AspNetCore.Mvc;
using StackBook.DAL.IRepository;

namespace StackBook.ViewComponents
{
    public class SliderCategoryViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public SliderCategoryViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // Có thể lấy dữ liệu từ database
        public IViewComponentResult Invoke()
        {
            var category = _unitOfWork.Category.GetAll();
            return View(category);
        }
    }
}
