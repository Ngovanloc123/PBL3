using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; } = Guid.NewGuid();
        public virtual ReturnOrder? ReturnOrder { get; set; }
        public virtual OrderDetail? OrderDetail { get; set; }
        public virtual Payment? Payment { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        [Required]
        public Guid DiscountId { get; set; }
        public virtual Discount? Discount { get; set; }

        [Required]
        public Guid ShippingAddressId { get; set; }
        public virtual ShippingAddress? ShippingAddress { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public double TotalPrice { get; set; }


        //  1. Pending
        //  2. Shipped
        //  3. Canceled
        //  4. Delivered
        //  5. return
        [Required]
        [Range(1, 5, ErrorMessage = "The type must be between 1 and 5.")]
        public int Status { get; set; } 
    }
}
