using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StackBook.ViewModels
{
    public class BookVM
    {
        public Guid? BookId { get; set; }

        [Required(ErrorMessage = "Book title is required.")]
        [StringLength(255, ErrorMessage = "Book title cannot exceed 255 characters.")]
        public string? BookTitle { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
        public int Stock { get; set; }

        public string? ImageURL { get; set; }

        [Required(ErrorMessage = "Created date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime CreatedBook { get; set; }

        public IEnumerable<SelectListItem>? Categories { get; set; }
        public IEnumerable<SelectListItem>? Authors { get; set; }

        [Required(ErrorMessage = "At least one category must be selected.")]
        public List<Guid> SelectedCategoryIds { get; set; } = new List<Guid>();

        [Required(ErrorMessage = "At least one author must be selected.")]
        public List<Guid> SelectedAuthorIds { get; set; } = new List<Guid>();
    }
}


