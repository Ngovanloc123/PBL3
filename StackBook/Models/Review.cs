using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class Review
    {
        [Key]
        public Guid ReviewId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        [Required]
        public Guid BookId { get; set; }
        public virtual Book? Book { get; set; }
        public Guid? OrderId { get; set; }
        public virtual Order? Order { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        public int Rating { get; set; }

        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
