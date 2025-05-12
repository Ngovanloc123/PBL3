using StackBook.Models;

namespace StackBook.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Book> Books { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
