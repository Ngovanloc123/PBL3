using StackBook.Models;
using static StackBook.ViewModels.BookWithAuthors;

namespace StackBook.ViewModels
{
    public class CategoryWithBooksViewModel
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<BookWithAuthors> BookWithAuthors { get; set; }
    }

    
}
