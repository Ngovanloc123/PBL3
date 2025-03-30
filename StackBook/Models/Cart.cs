using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StackBook.Models
{
    public class Cart
    {
        [Key]
        public Guid CartId { get; set; } = Guid.NewGuid();
        public virtual CartDetail? CartDetail { get; set; }

        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        public DateTime CreatedCart { get; set; }
    }
}
