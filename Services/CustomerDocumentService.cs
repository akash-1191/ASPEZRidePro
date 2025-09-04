using EZRide_Project.DTO;
using EZRide_Project.Helpers;
using EZRide_Project.Model.Entities;
using EZRide_Project.Model;
using EZRide_Project.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Services
{
    public class CustomerDocumentService : ICustomerDocumentService
    {
        private readonly ICustomerDocumentRepository _repository;
        private readonly IWebHostEnvironment _env;

        public CustomerDocumentService(ICustomerDocumentRepository repository, IWebHostEnvironment env)
        {
            _repository = repository;
            _env = env;
        }
        //Add documnet
        public async Task<bool> HasUserUploadedAnyDocumentsAsync(int userId)
        {
            return await _repository.HasUserUploadedAnyDocumentsAsync(userId);
        }


        public async Task AddAsync(CustomerDocumentCreateDTO dto)
        {
            var folderPath = Path.Combine(_env.WebRootPath, "Upload_image", "CustomerDocument");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // File paths
            string? ageProofPath = dto.AgeProof != null ? await SaveFileAsync(dto.AgeProof, folderPath) : null;
            string? addressProofPath = dto.AddressProof != null ? await SaveFileAsync(dto.AddressProof, folderPath) : null;
            string? dlImagePath = dto.DLImage != null ? await SaveFileAsync(dto.DLImage, folderPath) : null;

            // Pehle check karo ki record already exist karta hai ya nahi
            var existingDoc = await _repository.GetByUserIdRawAsync(dto.UserId); // NOTE: This should return Entity not DTO

            if (existingDoc != null)
            {
                // Update existing record
                if (ageProofPath != null) existingDoc.AgeProofPath = ageProofPath;
                if (addressProofPath != null) existingDoc.AddressProofPath = addressProofPath;
                if (dlImagePath != null) existingDoc.DLImagePath = dlImagePath;

                existingDoc.CreatedAt = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(dto.Status) &&
                    Enum.TryParse(dto.Status, true, out CustomerDocument.DocumentStatus parsedStatus))
                {
                    existingDoc.Status = parsedStatus;
                }

                await _repository.UpdateAsync(existingDoc); // You must implement UpdateAsync in repository
            }
            else
            {
                // Add new record
                var document = new CustomerDocument
                {
                    UserId = dto.UserId,
                    AgeProofPath = ageProofPath,
                    AddressProofPath = addressProofPath,
                    DLImagePath = dlImagePath,
                    CreatedAt = DateTime.UtcNow,
                    Status = CustomerDocument.DocumentStatus.Active
                };

                if (!string.IsNullOrEmpty(dto.Status) &&
                    Enum.TryParse(dto.Status, true, out CustomerDocument.DocumentStatus parsedStatus))
                {
                    document.Status = parsedStatus;
                }

                await _repository.AddAsync(document);
            }
        }

        //get all document
        public async Task<IEnumerable<CustomerDocumentReadDTO>> GetAllAsync()
        {
            var documents = await _repository.GetAllAsync();

            return documents.Select(d => new CustomerDocumentReadDTO
            {
                DocumentId = d.DocumentId,
                UserId = d.UserId,
                AgeProofPath = d.AgeProofPath,
                AddressProofPath = d.AddressProofPath,
                DLImagePath = d.DLImagePath,
                Status = d.Status.ToString(),
                CreatedAt = d.CreatedAt
            });
        }

        //get document by user id
        public async Task<CustomerDocumentReadDTO> GetByUserIdAsync(int userId)
        {
            var document = await _repository.GetByUserIdAsync(userId);
            if (document == null) return null;

            return new CustomerDocumentReadDTO
            {
                DocumentId = document.DocumentId,
                UserId = document.UserId,
                AgeProofPath = document.AgeProofPath,
                AddressProofPath = document.AddressProofPath,
                DLImagePath = document.DLImagePath,
                CreatedAt = document.CreatedAt,
                Status = document.Status.ToString()
            };
        }
     

        // Helper to remove file from server
        private void DeleteFileIfExists(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return;

            var fullPath = Path.Combine(_env.WebRootPath, relativePath.Replace("/", "\\"));

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        //validatio to check file type and drive 
        private async Task<string> SaveFileAsync(IFormFile file, string folderPath)
        {

            if (file == null || file.Length == 0)
                throw new Exception("File is empty or missing.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(ext))
                throw new Exception("Invalid file type. Only JPG, PNG, and PDF files are allowed.");

            if (file.Length > 5 * 1024 * 1024) // 5 MB
                throw new Exception("File size cannot exceed 5MB.");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }


            return Path.Combine("Upload_image", "CustomerDocument", fileName).Replace("\\", "/");
        }



        public async Task UpdateDocumentFieldToNullAndDeleteFileAsync(int userId, string fieldName)
        {
            var document = await _repository.GetByUserIdAsync(userId);
            if (document == null) return;

            string filePath = null;

            switch (fieldName.ToLower())
            {
                case "ageproof":
                    filePath = document.AgeProofPath;
                    break;
                case "addressproof":
                    filePath = document.AddressProofPath;
                    break;
                case "dlimage":
                    filePath = document.DLImagePath;
                    break;
                default:
                    throw new ArgumentException("Invalid field name");
            }

            // 1. Null in DB
            await _repository.UpdateDocumentFieldToNullAsync(userId, fieldName);

            // 2. Delete file from server
            if (!string.IsNullOrEmpty(filePath))
            {
                var fullPath = Path.Combine(_env.WebRootPath, filePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
        }
    }
}