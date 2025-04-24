using Microsoft.AspNetCore.Mvc;
using StackBook.Models;
using StackBook.ViewModels;

namespace StackBook.ViewComponents
{
    public class BooksViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<Book> books)
        {
            return View(books);
        }
    }
}
