using System.ComponentModel.DataAnnotations;

namespace StackBook.Models
{
    public class SBook
    {
        public Guid TitleBook { get; set; }
        public Guid Author { get; set; }
        public Guid Category { get; set; }
    }
}
