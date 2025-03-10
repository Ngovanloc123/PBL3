using StackBook.Models;

namespace StackBook.ViewModels
{
    public class CategoryWithBooksViewModel
    {
        public string CategoryName { get; set; }
        public List<BookWithAuthors> BookWithAuthors { get; set; }
    }

    public class BookWithAuthors
    {
        public Book Book { get; set; }
        public List<Author> Authors { get; set; }
    }
}
