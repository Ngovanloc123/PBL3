using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class Notification
    {
        [Key]
        public Guid NotificationId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        [Required]
        public string? Message { get; set; }
        public bool Status { get; set; }
    }
}
