using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Helpers;
using EZRide_Project.Model;
using EZRide_Project.Model.Entities;
using EZRide_Project.Repositories;

namespace EZRide_Project.Services
{
    public class OwnerDocumentService : IOwnerDocumentService
    {
        private readonly IOwnerDocumentRepository _repo;
        private readonly Cloudinary _cloudinary;

        public OwnerDocumentService(IOwnerDocumentRepository repo, Cloudinary cloudinary)
        {
            _repo = repo;
            _cloudinary = cloudinary;
        }

        public async Task<ApiResponseModel> AddOwnerDocumentAsync(int ownerId, AddOwnerDocumentDTO dto)
        {
            if (dto.DocumentFile == null)
                return ApiResponseHelper.Fail("Document File is required.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var ext = Path.GetExtension(dto.DocumentFile.FileName).ToLower();

            if (!allowedExtensions.Contains(ext))
                return ApiResponseHelper.Fail("Only JPG, PNG, and PDF allowed.");

            if (dto.DocumentFile.Length > 5 * 1024 * 1024)
                return ApiResponseHelper.Fail("Max file size 5 MB.");

            using var stream = dto.DocumentFile.OpenReadStream();

            var uploadParams = new RawUploadParams()
            {
                File = new FileDescription(dto.DocumentFile.FileName, stream),
                Folder = "EZRide/OwnerDocuments",
                PublicId = Guid.NewGuid().ToString()
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.Error != null)
                return ApiResponseHelper.Fail(uploadResult.Error.Message);

            string cloudUrl = uploadResult.SecureUrl.AbsoluteUri;
            string publicId = uploadResult.PublicId;

            var existingDoc = await _repo.GetOwnerDocumentByType(ownerId, dto.DocumentType);

            if (existingDoc != null)
            {
                if (!string.IsNullOrEmpty(existingDoc.PublicId))
                    await _cloudinary.DestroyAsync(new DeletionParams(existingDoc.PublicId));

                existingDoc.DocumentPath = cloudUrl;
                existingDoc.PublicId = publicId;
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
                    DocumentPath = cloudUrl,
                    PublicId = publicId,
                    Status = OwnerDocument.DocumentStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await _repo.AddAsync(doc);
            }

            await _repo.SaveChangesAsync();
            return ApiResponseHelper.Success("Document uploaded successfully!");
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
                PublicId = d.PublicId ?? string.Empty,
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

            // Cloudinary delete
            if (!string.IsNullOrEmpty(doc.PublicId))
            {
                var delParams = new DeletionParams(doc.PublicId);
                await _cloudinary.DestroyAsync(delParams);
            }

            _repo.Delete(doc);
            await _repo.SaveChangesAsync();

            return ApiResponseHelper.Success("Document deleted successfully.");
        }


        // Update document
        public async Task<ApiResponseModel> UpdateOwnerDocumentAsync(int ownerId, updateOwnerDocumentDTO dto)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var maxFileSize = 5 * 1024 * 1024; // 5 MB

            var doc = await _repo.GetByIdAsync(dto.DocumentId);
            if (doc == null || doc.OwnerId != ownerId)
                return ApiResponseHelper.Fail("Document not found or access denied.");

            if (dto.DocumentFile != null)
            {
                var ext = Path.GetExtension(dto.DocumentFile.FileName).ToLower();
                if (!allowedExtensions.Contains(ext))
                    return ApiResponseHelper.Fail("Only JPG, PNG, and PDF files are allowed.");

                if (dto.DocumentFile.Length > maxFileSize)
                    return ApiResponseHelper.Fail("File size cannot exceed 5 MB.");

                // Delete old from Cloudinary
                if (!string.IsNullOrEmpty(doc.PublicId))
                {
                    var deleteParams = new DeletionParams(doc.PublicId);
                    await _cloudinary.DestroyAsync(deleteParams);
                }

                using var stream = dto.DocumentFile.OpenReadStream();
                var uploadParams = new RawUploadParams()
                {
                    File = new FileDescription(dto.DocumentFile.FileName, stream),
                    Folder = "EZRide/OwnerDocuments"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.Error != null)
                    return ApiResponseHelper.Fail(uploadResult.Error.Message);

                doc.DocumentPath = uploadResult.SecureUrl.AbsoluteUri;
                doc.PublicId = uploadResult.PublicId;
            }

            doc.DocumentType = Enum.Parse<OwnerDocument.documentType>(dto.DocumentType, true);
            doc.CreatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(doc);
            await _repo.SaveChangesAsync();

            return ApiResponseHelper.Success("Document updated successfully!");
        }


    }
}

