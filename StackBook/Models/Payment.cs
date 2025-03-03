using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StackBook.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        [Required]
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        public string PaymentMethod { get; set; }
        [Required]
        public string PaymentStatus { get; set; }

        public string Transaction { get; set; }
        public DateTime CreatedPayment { get; set; }
    }
}
