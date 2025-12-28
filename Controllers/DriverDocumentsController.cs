using System;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
        private readonly Cloudinary _cloudinary;

        public DriverDocumentsController(ApplicationDbContext context, IWebHostEnvironment env, Cloudinary cloudinary)
        {
            _context = context;
            _env = env;
            _cloudinary = cloudinary;

        }

        // ===========================
        // ADD DRIVER DOCUMENT
        // ===========================
        [HttpPost("addDriverDocument")]
        public async Task<IActionResult> AddDriverDocument([FromForm] AddDriverDocumentDto dto)
        {
            if (dto.DocumentFile == null || dto.DocumentFile.Length == 0)
                return BadRequest("Document file is required.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var ext = Path.GetExtension(dto.DocumentFile.FileName).ToLower();

            if (!allowedExtensions.Contains(ext))
                return BadRequest("Only JPG, PNG, and PDF files allowed.");

            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(dto.DocumentFile.FileName, dto.DocumentFile.OpenReadStream()),
                Folder = "EZRide/DriverDocuments"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            var document = new DriverDocuments
            {
                DriverId = dto.DriverId,
                DocumentType = dto.DocumentType,
                DocumentPath = uploadResult.SecureUrl.ToString(),
                PublicId = uploadResult.PublicId,
                Status = DriverDocuments.DocumentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.DriverDocuments.Add(document);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Driver document uploaded successfully",
                url = document.DocumentPath
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

            if (!string.IsNullOrEmpty(document.PublicId))
            {
                await _cloudinary.DestroyAsync(new DeletionParams(document.PublicId));
            }

            _context.DriverDocuments.Remove(document);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Driver document deleted successfully" });
        }


    }
}
