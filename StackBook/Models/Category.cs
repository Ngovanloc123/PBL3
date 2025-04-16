using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class Category
    {
        [Key]
        public Guid CategoryId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        [Display(Name = "Category Name")]
        public string? CategoryName { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Display Order must be between 1-100")]
        public int DisplayOrder { get; set; }

        public virtual ICollection<BookCategory>? BookCategories { get; set; }
    }
}
