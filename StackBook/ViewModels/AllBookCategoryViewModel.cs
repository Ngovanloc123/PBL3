using StackBook.Models;
using static StackBook.ViewModels.BookWithAuthors;

namespace StackBook.ViewModels
{
    public class AllBookCategoryViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Book> Books { get; set; }
    }
}
