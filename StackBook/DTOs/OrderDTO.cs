using System.ComponentModel.DataAnnotations;
namespace StackBook.DTOs
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty; 
        public double TotalPrice { get; set; }
        public int Status { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = new List<OrderDetailDto>();
        public string ShippingAddress { get; set; } = string.Empty;
    }
    public class OrderDetailDto
    {
        public Guid OrderDetailId { get; set; }
        public Guid BookId { get; set; }
        public string BookTitle { get; set; }  = string.Empty;
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }
    }

}