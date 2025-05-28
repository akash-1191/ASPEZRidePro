using EZRide_Project.DTO;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EZRide_Project.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CustomerDocumentController : ControllerBase
    {
        private readonly ICustomerDocumentService _documentService;
        private readonly IWebHostEnvironment _env;

        public CustomerDocumentController(ICustomerDocumentService documentService, IWebHostEnvironment env)
        {
            _documentService = documentService;
            _env = env;
        }

        [HttpPost("uploadCustomerDocument")]
        [Authorize]
        public async Task<IActionResult> Upload([FromForm] CustomerDocumentCreateDTO dto)
        {
            if (dto.AgeProof == null || dto.AddressProof == null)
                return BadRequest("Required files are missing");

            await _documentService.AddAsync(dto);
            return Ok(new { Message = "Document uploaded successfully" });
        }


        [HttpGet("GetAllCustomerDocument")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var documents = await _documentService.GetAllAsync();
            return Ok(documents);
        }

        [HttpGet("GetDocumnetByUserId{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var doc = await _documentService.GetByIdAsync(id);
            if (doc == null) return NotFound();
            return Ok(doc);
        }


        [HttpDelete("DocumentDelete{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            await _documentService.DeleteAsync(id);
            return Ok(new { Message = "Document deleted successfully" });
        }
    }
}
