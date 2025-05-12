using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EZRide_Project.Data;
using EZRide_Project.Model.Entities;
using EZRide_Project.DTO;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContactsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Contacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactDTO>>> GetContacts()
        {
            var contacts = await _context.Contacts
                .Select(c => new ContactDTO
                {
                    ContactId = c.ContactId,
                    UserId = c.UserId,
                    Subject = c.Subject,
                    Message = c.Message,
                    Status = c.Status.ToString(),
                    CreatedAt = c.CreatedAt
                }).ToListAsync();

            return Ok(contacts);
        }

        // GET: api/Contacts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContactDTO>> GetContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            var dto = new ContactDTO
            {
                ContactId = contact.ContactId,
                UserId = contact.UserId,
                Subject = contact.Subject,
                Message = contact.Message,
                Status = contact.Status.ToString(),
                CreatedAt = contact.CreatedAt
            };

            return Ok(dto);
        }

        // POST: api/Contacts
        [HttpPost]
        public async Task<ActionResult<ContactDTO>> PostContact(ContactDTO dto)
        {
            var contact = new Contact
            {
                UserId = dto.UserId,
                Subject = dto.Subject,
                Message = dto.Message,
                Status = Enum.Parse<Contact.ContactStatus>(dto.Status),
                CreatedAt = DateTime.Now
            };

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            dto.ContactId = contact.ContactId;
            dto.CreatedAt = contact.CreatedAt;

            return CreatedAtAction(nameof(GetContact), new { id = contact.ContactId }, dto);
        }

        // PUT: api/Contacts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact(int id, ContactDTO dto)
        {
            if (id != dto.ContactId)
                return BadRequest();

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
                return NotFound();

            contact.Subject = dto.Subject;
            contact.Message = dto.Message;
            contact.Status = Enum.Parse<Contact.ContactStatus>(dto.Status);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Contacts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
                return NotFound();

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
