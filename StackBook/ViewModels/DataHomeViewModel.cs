using StackBook.Models;
using static StackBook.ViewModels.BookWithAuthors;

namespace StackBook.ViewModels
{
    public class DataHomeViewModel
    {
        public List<Category> Categories { get; set; }
        public List<BookWithAuthors> BookWithAuthors { get; set; }
    }
}
