using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class ReturnOrder
    {
        [Key]
        public Guid ReturnOrderId { get; set; } = Guid.NewGuid();
        public virtual ReturnOrderDetail? ReturnOrderDetail { get; set; }

        [Required]
        public Guid OrderId { get; set; }
        public virtual Order? Order { get; set; }

        [Required]
        public string? reason { get; set; }

        [Required]
        public DateTime CreatedReturn { get; set; }
    }
}
