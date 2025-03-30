using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class Category
    {
        [Key]
        [Required]
        public Guid CategoryId { get; set; } = Guid.NewGuid();


        [Required(ErrorMessage = "Category name is required!")]
        public string? CategoryName { get; set; }

        public virtual ICollection<BookCategory>? BookCategories { get; set; }
    }
}
