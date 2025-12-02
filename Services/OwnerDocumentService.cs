using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Helpers;
using EZRide_Project.Migrations;
using EZRide_Project.Model;
using EZRide_Project.Model.Entities;
using EZRide_Project.Repositories;

namespace EZRide_Project.Services
{
    public class OwnerDocumentService : IOwnerDocumentService
    {
        private readonly IOwnerDocumentRepository _repo;
        private readonly IWebHostEnvironment _env;

        public OwnerDocumentService(IOwnerDocumentRepository repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }

        private string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
        private long maxFileSize = 5 * 1024 * 1024; // 5 MB

        // Add or update document
        public async Task<ApiResponseModel> AddOwnerDocumentAsync(int ownerId, AddOwnerDocumentDTO dto)
        {
            if (dto.DocumentFile == null)
                return ApiResponseHelper.Fail("Please provide a document to upload.");

            var ext = Path.GetExtension(dto.DocumentFile.FileName).ToLower();
            if (!allowedExtensions.Contains(ext))
                return ApiResponseHelper.Fail("Only JPG, PNG, or PDF files are allowed.");
            if (dto.DocumentFile.Length > maxFileSize)
                return ApiResponseHelper.Fail("File size cannot exceed 5 MB.");

            // Folder name inside wwwroot
            var folder = "Upload_image/OwnerDocument";
            var fileName = $"{Guid.NewGuid()}{ext}";

            // Actual physical path
            var physicalPath = Path.Combine(_env.WebRootPath, folder, fileName);

            // If directory not exist → create
            var directory = Path.GetDirectoryName(physicalPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await dto.DocumentFile.CopyToAsync(stream);
            }

            // Save only RELATIVE PATH in DB
            var relativePath = "/" + folder + "/" + fileName;

            var existingDoc = await _repo.GetOwnerDocumentByType(ownerId, dto.DocumentType);
            if (existingDoc != null)
            {
                // Delete old file
                if (!string.IsNullOrEmpty(existingDoc.DocumentPath))
                {
                    var oldPhysicalPath = Path.Combine(_env.WebRootPath, existingDoc.DocumentPath.TrimStart('/'));
                    if (File.Exists(oldPhysicalPath))
                        File.Delete(oldPhysicalPath);
                }

                existingDoc.DocumentPath = relativePath;
                existingDoc.Status = OwnerDocument.DocumentStatus.Pending;
                existingDoc.CreatedAt = DateTime.UtcNow;

                await _repo.UpdateAsync(existingDoc);
            }
            else
            {
                var doc = new OwnerDocument
                {
                    OwnerId = ownerId,
                    DocumentType = Enum.Parse<OwnerDocument.documentType>(dto.DocumentType, true),
                    DocumentPath = relativePath,
                    Status = OwnerDocument.DocumentStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await _repo.AddAsync(doc);
            }

            await _repo.SaveChangesAsync();
            return ApiResponseHelper.Success("Document uploaded successfully.");
        }


        // Get all documents
        public async Task<List<OwnerDocumentDTO>> GetOwnerDocumentsAsync(int ownerId)
        {
            var docs = await _repo.GetOwnerDocumentsAsync(ownerId);
            return docs.Select(d => new OwnerDocumentDTO
            {
                DocumentId = d.DocumentId,
                DocumentType = d.DocumentType.ToString(),
                DocumentPath = d.DocumentPath,
                Status = d.Status.ToString(),
                Reason = d.Reason,
                CreatedAt = d.CreatedAt
            }).ToList();
        }

        // Delete document
        public async Task<ApiResponseModel> DeleteOwnerDocumentAsync(int ownerId, int documentId)
        {
            var doc = await _repo.GetByIdAsync(documentId);
            if (doc == null || doc.OwnerId != ownerId)
                return ApiResponseHelper.Fail("Document not found or access denied.");

            if (!string.IsNullOrEmpty(doc.DocumentPath) && File.Exists(doc.DocumentPath))
            {
                File.Delete(doc.DocumentPath);
            }

            _repo.Delete(doc);
            await _repo.SaveChangesAsync();

            return ApiResponseHelper.Success("Document deleted successfully.");
        }

        // Update document
        public async Task<ApiResponseModel> UpdateOwnerDocumentAsync(int ownerId, updateOwnerDocumentDTO dto)
        {
            var doc = await _repo.GetByIdAsync(dto.DocumentId);
            if (doc == null || doc.OwnerId != ownerId)
                return ApiResponseHelper.Fail("Document not found or access denied.");

            // Update file if provided
            if (dto.DocumentFile != null)
            {
                var ext = Path.GetExtension(dto.DocumentFile.FileName).ToLower();
                if (!allowedExtensions.Contains(ext))
                    return ApiResponseHelper.Fail("Only JPG, PNG, or PDF files are allowed.");
                if (dto.DocumentFile.Length > maxFileSize)
                    return ApiResponseHelper.Fail("File size cannot exceed 5 MB.");

                var folderPath = Path.Combine(_env.WebRootPath, "Upload_image", "OwnerDocument");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, $"{Guid.NewGuid()}{ext}");
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.DocumentFile.CopyToAsync(stream);
                }

                // Delete old file
                if (!string.IsNullOrEmpty(doc.DocumentPath) && File.Exists(doc.DocumentPath))
                    File.Delete(doc.DocumentPath);

                doc.DocumentPath = filePath;
            }

            doc.DocumentType = Enum.Parse<OwnerDocument.documentType>(dto.DocumentType, true);
            doc.CreatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(doc);
            await _repo.SaveChangesAsync();

            return ApiResponseHelper.Success("Document updated successfully.");
        }
    }
}

