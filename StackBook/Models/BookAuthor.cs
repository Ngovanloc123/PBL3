using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class BookAuthor
    {
        [Required]
        public Guid BookId { get; set; }
        public virtual Book? Book { get; set; }
        [Required]
        public Guid AuthorId { get; set; }
        public virtual Author? Author { get; set; }
    }
}
