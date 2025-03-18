using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class ReturnOrderDetail
    {
        [Key]
        public Guid ReturnOrderDetailId { get; set; } = Guid.NewGuid();
        [Required]
        public Guid ReturnOrderId { get; set; }
        public virtual ICollection<ReturnOrder>? ReturnOrders { get; set; }
        [Required]
        public Guid BookId { get; set; }
        public virtual Book? Book { get; set; }
        public int Quantity { get; set; }
    }
}
