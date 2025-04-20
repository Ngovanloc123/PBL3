using Microsoft.AspNetCore.Mvc;
using StackBook.Models;
using StackBook.ViewModels;

namespace StackBook.ViewComponents
{
    public class BooksViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<BookWithAuthors> bookWithAuthor)
        {
            return View(bookWithAuthor);
        }
    }
}
