using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerDocumentController : ControllerBase
    {
        private readonly IOwnerDocumentService _documentService;

        public OwnerDocumentController(IOwnerDocumentService documentService)
        {
            _documentService = documentService;
        }

        // ADD Document
        [Authorize(Roles = "OwnerVehicle")]
        [HttpPost("add")]
        public async Task<IActionResult> Upload([FromForm] AddOwnerDocumentDTO dto)
        {
            if (dto.DocumentFile == null)
                return BadRequest("Please provide a document to upload.");

            // Allowed file types
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var extension = Path.GetExtension(dto.DocumentFile.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Only JPG, PNG, or PDF files are allowed.");

            // Max size 5 MB
            if (dto.DocumentFile.Length > 5 * 1024 * 1024)
                return BadRequest("File size cannot exceed 5 MB.");

            try
            {
                int ownerId = int.Parse(User.FindFirst("UserId").Value);
                await _documentService.AddOwnerDocumentAsync(ownerId, dto);
                return Ok(new { Message = "Document uploaded or updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal server error.", Details = ex.Message });
            }
        }

        // GET All Documents
        [Authorize(Roles = "OwnerVehicle")]
        [HttpGet("get")]
        public async Task<IActionResult> GetOwnerDocuments()
        {
            int ownerId = int.Parse(User.FindFirst("UserId").Value);
            var result = await _documentService.GetOwnerDocumentsAsync(ownerId);
            return Ok(result);
        }

        // DELETE Document
        [Authorize(Roles = "OwnerVehicle")]
        [HttpDelete("delete/{documentId}")]
        public async Task<IActionResult> DeleteOwnerDocument(int documentId)
        {
            int ownerId = int.Parse(User.FindFirst("UserId").Value);
            var result = await _documentService.DeleteOwnerDocumentAsync(ownerId, documentId);
            return StatusCode(result.StatusCode, result);
        }

        // UPDATE Document
        [Authorize(Roles = "OwnerVehicle")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateOwnerDocument([FromForm] updateOwnerDocumentDTO dto)
        {
            int ownerId = int.Parse(User.FindFirst("UserId").Value);
            var result = await _documentService.UpdateOwnerDocumentAsync(ownerId, dto);
            return StatusCode(result.StatusCode, result);
        }
    }
}
