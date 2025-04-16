using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StackBook.Models
{
    public class Cart
    {
        [Key]
        public Guid CartId { get; set; } = Guid.NewGuid();
        public virtual ICollection<CartBook> CartBooks { get; set; }

        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
