using System.ComponentModel.DataAnnotations;

namespace Library.DTOs
{
    public class BookCreateDTO
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public int AuthorId { get; set; }
    }
}
