namespace StackBook.Models
{
    public class OrderHistory
    {
        public Guid OrderHistoryId { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }
        public int Status { get; set; }
        public DateTime createdStatus { get; set; }
    }
}
