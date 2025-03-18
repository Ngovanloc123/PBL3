using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class BookCategory
    {
        [Required]
        public Guid BookId { get; set; }
        public virtual Book? Book { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        public virtual Category? Category { get; set; }
    }
}
