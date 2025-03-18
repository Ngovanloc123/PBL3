using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class Book
    {
        [Key]
        [Required]
        public Guid BookId { get; set; } = Guid.NewGuid();
        public virtual ICollection<CartDetail>? CartDetail { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }


        [Required(ErrorMessage = "Product name is required.")]
        public string? BookTitle { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Product image URL is required.")]
        [Url(ErrorMessage = "Invalid URL format.")]
        public string? ImageURL { get; set; }

        [Required]
        public DateTime CreatedBook { get; set; }

        public virtual ICollection<BookAuthor>? BookAuthors { get; set; }
        public virtual ICollection<BookCategory>? BookCategories { get; set; }
    }
}
