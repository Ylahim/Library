using System.ComponentModel.DataAnnotations;

namespace Library.DTOs
{
    public class AuthorCreateDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
    }
}
