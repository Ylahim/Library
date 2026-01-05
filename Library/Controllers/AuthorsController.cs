using Library.Data;
using Library.DTOs;
using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public AuthorsController(LibraryDbContext context)
        {
            _context = context;
        }

        //api/authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorReadDTO>>> GetAuthors()
        {
            var authors = await _context.Authors
                .Select(a => new AuthorReadDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    BirthDate = a.BirthDate
                })
                .ToListAsync();

            return Ok(authors);
        }

        //api/authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorReadDTO>> GetAuthor(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            var authorDto = new AuthorReadDTO
            {
                Id = author.Id,
                Name = author.Name,
                BirthDate = author.BirthDate,
                Books = author.Books.Select(b => new BookReadDTO
                {
                    Id = b.Id,
                    Title = b.Title,
                    PublicationYear = b.PublicationYear,
                    AuthorId = b.AuthorId,
                    AuthorName = author.Name
                }).ToList()
            };

            return Ok(authorDto);
        }

        //api/authors
        [HttpPost]
        public async Task<ActionResult<AuthorReadDTO>> PostAuthor(AuthorCreateDTO authorDto)
        {
            var author = new Author
            {
                Name = authorDto.Name,
                BirthDate = authorDto.BirthDate
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            var readDto = new AuthorReadDTO
            {
                Id = author.Id,
                Name = author.Name,
                BirthDate = author.BirthDate
            };

            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, readDto);
        }

        //api/authors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, AuthorCreateDTO authorDto)
        {
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            author.Name = authorDto.Name;
            author.BirthDate = authorDto.BirthDate;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
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

        //api/authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}