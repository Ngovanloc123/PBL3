using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class OrderDetail
    {
        [Key]
        public Guid OrderDetailId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid OrderId { get; set; }
        public virtual ICollection<Order>? Order { get; set; }

        [Required]
        public Guid BookId { get; set; } 
        public virtual Book? Book { get; set; }

        [Required]
        public int quantity { get; set; }
    }
}
