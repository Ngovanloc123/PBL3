using StackBook.Models;

namespace StackBook.ViewModels
{
    public class BookWithAuthors
    {
        public Book Book { get; set; }
        public List<Author> Authors { get; set; }
        
    }
}
