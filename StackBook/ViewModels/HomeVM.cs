using StackBook.Models;

namespace StackBook.ViewModels
{

    public class BookRatingViewModel
    {
        public Book Book { get; set; } = new Book();
        public double AverageRating { get; set; }
    }
    public class HomeVM
    {
        public IEnumerable<BookRatingViewModel> NewReleaseBooks { get; set; }
        public IEnumerable<BookRatingViewModel> BestSellerBooks { get; set; }
        public IEnumerable<BookRatingViewModel> RecommendBooks { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public int? Page { get; set; }
    }
}