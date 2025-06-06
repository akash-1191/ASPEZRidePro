using EZRide_Project.DTO;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddContact([FromBody] ContactDTO contactDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _contactService.AddContactAsync(contactDto);
            if (result)
                return Ok(new { Message = "Contact added successfully." });

            return StatusCode(500, "Failed to add contact.");
        }


        [HttpGet("allContactDetails")]
        public async Task<IActionResult> GetAllContacts()
        {
            var contacts = await _contactService.GetAllContactsAsync();
            return Ok(contacts);
        }
    }
}

