using Microsoft.AspNetCore.Mvc;
using StackBook.Models;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.AspNetCore.Routing;
using StackBook.ViewModels;

namespace StackBook.ViewComponents
{
    public class BooksViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<BookRatingViewModel> books,  int? page)
        {
            int pageSize = 8;
            int pageNumber = page ?? 1;

            if (books == null)
                books = new List<BookRatingViewModel>();

            var pagedBooks = books.ToPagedList(pageNumber, pageSize);

            var query = HttpContext.Request.Query;
            var routeValues = new RouteValueDictionary();

            foreach (var key in query.Keys)
            {
                if (key != "page") 
                    routeValues[key] = query[key].ToString();
            }

            ViewBag.RouteValues = routeValues;

            ViewBag.ActionName = ViewContext.RouteData.Values["action"].ToString();

            return View(pagedBooks);
        }
    }
}
