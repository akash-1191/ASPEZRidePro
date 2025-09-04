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
            if (dto.AgeProof == null && dto.AddressProof == null && dto.DLImage == null)
                return BadRequest("Please  document must be provided.");

            try
            {
                await _documentService.AddAsync(dto);
                return Ok(new { Message = "Document uploaded or updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error.", Details = ex.Message });
            }
        }


        [HttpGet("checkDocumentsUploaded/{userId}")]
        [Authorize]
        public async Task<IActionResult> CheckDocumentsUploaded(int userId)
        {
            bool allUploaded = await _documentService.HasUserUploadedAnyDocumentsAsync(userId);
            return Ok(new { exists = allUploaded });
        }


        [HttpGet("GetAllCustomerDocument")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var documents = await _documentService.GetAllAsync();
            return Ok(documents);
        }

        [HttpGet("getCustomerDocument/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetCustomerDocument(int userId)
        {
            var doc = await _documentService.GetByUserIdAsync(userId);
            if (doc == null)
                return NotFound(new { Message = "No documents found for this user." });

            return Ok(doc);
        }



        [HttpPut("update-document-field-null/{userId}/{fieldName}")]
        [Authorize]
        public async Task<IActionResult> UpdateDocumentFieldToNull(int userId, string fieldName)
        {
            try
            {
                await _documentService.UpdateDocumentFieldToNullAndDeleteFileAsync(userId, fieldName);
                return Ok(new { message = "Document field cleared successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
