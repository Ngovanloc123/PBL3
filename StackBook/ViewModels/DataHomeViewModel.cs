using StackBook.Models;

namespace StackBook.ViewModels
{
    public class DataHomeViewModel
    {
        public List<MenuCategories> MenuCategories { get; set; }
        public List<Book> Books { get; set; }
    }

    public class MenuCategories
    {
        public string CategoryName { get; set; }
        public int Count { get; set; }
    }
}
