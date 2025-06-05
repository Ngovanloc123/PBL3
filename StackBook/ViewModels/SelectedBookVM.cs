using StackBook.Models;

namespace StackBook.ViewModels
{
    public class SelectedBookVM
    {
        public bool IsSelected { get; set; }
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
    }

    public class SelectedBook
    {
        public Book Book { get; set; }
        public int Quantity { get; set; }
    }

    public class CheckoutRequest
    {
        public User User { get; set; }
        public List<SelectedBook> SelectedBooks { get; set; }
        public ShippingAddress? shippingAddressDefault { get; set; }
        public string PaymentMethod { get; set; }
        public List<Discount> discounts { get; set; }
        public Guid DiscountId { get; set; }
    }
}