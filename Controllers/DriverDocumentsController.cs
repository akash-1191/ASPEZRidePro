using System;
using EZRide_Project.Data;
using EZRide_Project.DTO.Driver_DTO;
using EZRide_Project.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverDocumentsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DriverDocumentsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ===========================
        // ADD DRIVER DOCUMENT
        // ===========================
        [HttpPost("addDriverDocument")]
        public async Task<IActionResult> AddDriverDocument([FromForm] AddDriverDocumentDto dto)
        {
            if (dto.DocumentFile == null || dto.DocumentFile.Length == 0)
                return BadRequest("Document file is required");

            var folderPath = Path.Combine(_env.WebRootPath, "DriverDocuments");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}_{dto.DocumentFile.FileName}";
            var fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await dto.DocumentFile.CopyToAsync(stream);
            }

            var document = new DriverDocuments
            {
                DriverId = dto.DriverId,
                DocumentType = dto.DocumentType,
                DocumentPath = $"DriverDocuments/{fileName}",
                CreatedAt = DateTime.Now
            };

            _context.DriverDocuments.Add(document);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Driver document uploaded successfully"
            });
        }


        // ===========================
        // GET DRIVER DOCUMENTS
        // ===========================
        [HttpGet("getDriverDocuments/{driverId}")]
        public async Task<IActionResult> GetDriverDocuments(int driverId)
        {
            var documents = await _context.DriverDocuments
                .Where(d => d.DriverId == driverId)
                .Select(d => new
                {
                    d.DocumentId,
                    d.DocumentType,
                    d.DocumentPath,
                    d.CreatedAt
                })
                .ToListAsync();

            if (documents == null || documents.Count == 0)
                return Ok(new
                {
                    message = "No documents found for this driver",
                    data = new List<object>()
                });

            return Ok(new
            {
                message = "Driver documents fetched successfully",
                data = documents
            });
        }

        // ===========================
        // DELETE DRIVER DOCUMENT
        // ===========================
        [HttpDelete("deleteDriverDocument/{documentId}")]
        public async Task<IActionResult> DeleteDriverDocument(int documentId)
        {
            var document = await _context.DriverDocuments.FindAsync(documentId);
            if (document == null)
                return NotFound("Document not found");

            // Delete physical file
            if (!string.IsNullOrEmpty(document.DocumentPath))
            {
                var fullPath = Path.Combine(_env.WebRootPath, document.DocumentPath);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            _context.DriverDocuments.Remove(document);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Driver document deleted successfully"
            });
        }
    }
}
