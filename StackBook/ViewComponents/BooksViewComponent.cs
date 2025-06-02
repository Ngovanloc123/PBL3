using Microsoft.AspNetCore.Mvc;
using StackBook.Models;
using StackBook.ViewModels;
using X.PagedList.Extensions;

namespace StackBook.ViewComponents
{
    public class BooksViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<Book> books, int? page)
        {
            int pageSize = 8;
            int pageNumber = page ?? 1;
            var pagedBooks = books.ToPagedList(pageNumber, pageSize);

            return View(pagedBooks);
        }
    }
}
