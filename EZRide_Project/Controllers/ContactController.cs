using EZRide_Project.DTO;
using EZRide_Project.Helpers;
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
        private readonly WhatsAppService _whatsAppService;
        public ContactController(IContactService contactService,WhatsAppService whatsAppService)
        {
            _contactService = contactService;
            _whatsAppService = whatsAppService;
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

        //Send message for whatsapp

        [HttpPost("sendWhatsApp")]
        public async Task<IActionResult> SendWhatsApp([FromBody] WhatsAppSendDTO request)
        {
            if (string.IsNullOrEmpty(request.Phone) || string.IsNullOrEmpty(request.Message))
                return BadRequest("Phone and Message are required.");

            var isSent = await _whatsAppService.SendMessageAsync(request.Phone, request.Message);

            if (isSent)
                return Ok("WhatsApp message sent successfully.");
            else
                return StatusCode(500, "Failed to send WhatsApp message.");
        }
    }
}

