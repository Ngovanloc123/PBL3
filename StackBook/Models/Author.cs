using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class Author
    {
        [Key]
        public Guid AuthorId { get; set; } = Guid.NewGuid();
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();

        [Required(ErrorMessage = "Author name is required!")]
        [StringLength(100)]
        public string? AuthorName { get; set; } 
    }
}
