using Microsoft.AspNetCore.Mvc;

namespace StackBook.ViewModels
{
    public class BookDetailPageViewModel
    {
        public List<BookWithAuthors> AllBooks { get; set; }
        public BookWithAuthors? BookDetail { get; set; }
    }
}
