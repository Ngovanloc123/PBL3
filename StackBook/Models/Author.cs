using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class Author
    {
        [Key]
        public Guid AuthorId { get; set; } = Guid.NewGuid();
        public virtual ICollection<BookAuthor>? BookAuthors { get; set; }

        [Required(ErrorMessage = "Author name is required!")]
        public string? AuthorName { get; set; }
    }
}
