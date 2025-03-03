using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class Category
    {
        [Key]
        [Required]
        public int CategoryId { get; set; }


        [Required(ErrorMessage = "Category name is required!")]
        public string CategoryName { get; set; }

        public virtual ICollection<BookCategory> BookCategories { get; set; }
    }
}
