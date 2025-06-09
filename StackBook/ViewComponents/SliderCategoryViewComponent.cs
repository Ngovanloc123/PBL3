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

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var category = await _unitOfWork.Category.GetAllAsync("Books.Authors");
            return View(category);
        }
    }
}