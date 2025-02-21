using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
        public string imageURL { get; set; }
    }
}
