using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class Author
    {
        [Key]
        [Required]
        public int AuthorId { get; set; }
        public virtual ICollection<BookAuthor> BookAuthors { get; set; }

        [Required(ErrorMessage = "Author name is required!")]
        public string AuthorName { get; set; }
    }
}
