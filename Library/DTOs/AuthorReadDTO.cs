namespace Library.DTOs
{
    public class AuthorReadDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public IList<BookReadDTO> Books { get; set; } = new List<BookReadDTO>();
    }
}
