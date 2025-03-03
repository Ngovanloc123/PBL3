using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StackBook.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public virtual CartDetail CartDetail { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public DateTime CreatedCart { get; set; }
    }
}
