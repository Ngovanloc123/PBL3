using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class ReturnOrder
    {
        [Key]
        public int ReturnOrderId { get; set; }
        [Required]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        [Required]
        public int ShippingAddressId { get; set; }
        public virtual ShippingAddress ShippingAddress { get; set; }
        [Required]
        public string reason { get; set; }
        [Required]
        public DateTime CreatedReturn { get; set; }
    }
}
