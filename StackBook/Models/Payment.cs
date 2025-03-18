using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StackBook.Models
{
    public class Payment
    {
        [Key]
        public Guid PaymentId { get; set; }
        [Required]
        public Guid OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }
        [Required]
        public string? PaymentMethod { get; set; }
        [Required]
        public string? PaymentStatus { get; set; }
        [Required]
        public string? Transaction { get; set; }
        public DateTime CreatedPayment { get; set; }
    }
}
