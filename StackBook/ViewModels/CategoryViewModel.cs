using System.ComponentModel.DataAnnotations;

namespace StackBook.ViewModels
{
    public class CategoryViewModel
    {
        public Guid CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public int DisplayOrder { get; set; }
    }
}
