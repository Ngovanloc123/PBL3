
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using StackBook.Models;
using System.ComponentModel.DataAnnotations;

namespace StackBook.ViewModels
{
    public class BookDetailViewModel
    {
        [Key]
        public Guid BookId { get; set; }

        [Required(ErrorMessage = "Book title is required.")]
        [StringLength(200, ErrorMessage = "Book title cannot exceed 200 characters.")]
        [Display(Name = "Book Title")]
        public string? BookTitle { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        [Display(Name = "Price ($)")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
        [Display(Name = "Stock Quantity")]
        public int Stock { get; set; }

        [ValidateNever]
        public string? ImageURL { get; set; }

        [Required]
        [Display(Name = "Publication Date")]
        [DataType(DataType.Date)]
        public DateTime CreatedBook { get; set; }

        [Required]
        [Display(Name = "Authors")]
        public List<string> AuthorsName { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string? CategoryName { get; set; }
    }
}
