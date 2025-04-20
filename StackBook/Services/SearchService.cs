using StackBook.DAL.IRepository;
using StackBook.ViewModels;

namespace StackBook.Services
{
    public class SearchService
    {

        public List<BookWithAuthors> SearchBooks(List<BookWithAuthors> bookWithAuthors, String query)
        {
            var result = new List<BookWithAuthors>();
            foreach (var bookWithAuthor in bookWithAuthors)
            {
                // Thỏa mãn có chứa tên sách
                bool matchedByTitle = bookWithAuthor.Book.BookTitle.ToLower().Contains(query.ToLower());

                // // Thỏa mản có chứa Author
                bool matchedByAuthor = bookWithAuthor.Authors.Any(author =>
                    author.AuthorName.ToLower().Contains(query.ToLower())
                );

                if (matchedByTitle || matchedByAuthor)
                {
                    result.Add(bookWithAuthor);
                }


            }
            return result;
        }
    }
}
