using Library.Data;
using Library.DTOs;
using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public BooksController(LibraryDbContext context)
        {
            _context = context;
        }

        //api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookReadDTO>>> GetBooks()
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Select(b => new BookReadDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    PublicationYear = b.PublicationYear,
                    AuthorId = b.AuthorId,
                    AuthorName = b.Author.Name
                })
                .ToListAsync();

            return Ok(books);
        }

        //api/books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookReadDTO>> GetBook(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            var bookDto = new BookReadDTO
            {
                Id = book.Id,
                Title = book.Title,
                PublicationYear = book.PublicationYear,
                AuthorId = book.AuthorId,
                AuthorName = book.Author.Name
            };

            return Ok(bookDto);
        }

        //api/books
        [HttpPost]
        public async Task<ActionResult<BookReadDTO>> PostBook(BookCreateDTO bookDto)
        {
            var author = await _context.Authors.FindAsync(bookDto.AuthorId);
            if (author == null)
            {
                return BadRequest("Invalid AuthorId");
            }

            var book = new Book
            {
                Title = bookDto.Title,
                PublicationYear = bookDto.PublicationYear,
                AuthorId = bookDto.AuthorId
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var readDto = new BookReadDTO
            {
                Id = book.Id,
                Title = book.Title,
                PublicationYear = book.PublicationYear,
                AuthorId = book.AuthorId,
                AuthorName = author.Name
            };

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, readDto);
        }

        //api/books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, BookCreateDTO bookDto)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            if (book.AuthorId != bookDto.AuthorId)
            {
                if (!await _context.Authors.AnyAsync(a => a.Id == bookDto.AuthorId))
                {
                    return BadRequest("Invalid AuthorId");
                }
            }

            book.Title = bookDto.Title;
            book.PublicationYear = bookDto.PublicationYear;
            book.AuthorId = bookDto.AuthorId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        //api/books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}