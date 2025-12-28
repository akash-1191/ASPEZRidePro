using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EZRide_Project.DTO;
using EZRide_Project.Helpers;
using EZRide_Project.Model;
using EZRide_Project.Model.Entities;
using EZRide_Project.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Services
{
    public class CustomerDocumentService : ICustomerDocumentService
    {
        private readonly ICustomerDocumentRepository _repository;
        private readonly Cloudinary _cloudinary;

        public CustomerDocumentService(ICustomerDocumentRepository repository, Cloudinary cloudinary)
        {
            _repository = repository;
            _cloudinary = cloudinary;
        }

        public async Task AddAsync(CustomerDocumentCreateDTO dto)
        {
            var existingDoc = await _repository.GetByUserIdRawAsync(dto.UserId);

            if (existingDoc == null)
            {
                existingDoc = new CustomerDocument
                {
                    UserId = dto.UserId,
                    Status = CustomerDocument.DocumentStatus.Active,
                    CreatedAt = DateTime.UtcNow
                };
                await _repository.AddAsync(existingDoc);
            }

            // 🔹 Age Proof upload
            if (dto.AgeProof != null)
                await UploadDocument(dto.AgeProof, "EZRide/Documents/AgeProof",
                    (url, id) => { existingDoc.AgeProofPath = url; existingDoc.AgeProofPublicId = id; });

            // 🔹 Address Proof upload
            if (dto.AddressProof != null)
                await UploadDocument(dto.AddressProof, "EZRide/Documents/AddressProof",
                    (url, id) => { existingDoc.AddressProofPath = url; existingDoc.AddressProofPublicId = id; });

            // 🔹 DL Upload
            if (dto.DLImage != null)
                await UploadDocument(dto.DLImage, "EZRide/Documents/DL",
                    (url, id) => { existingDoc.DLImagePath = url; existingDoc.DLImagePublicId = id; });

            existingDoc.CreatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(existingDoc);
        }

        private async Task UploadDocument(IFormFile file, string folder,
            Action<string, string> updateCallback)
        {
            string ext = Path.GetExtension(file.FileName).ToLower();
            if (!new[] { ".jpg", ".jpeg", ".png", ".pdf" }.Contains(ext))
                throw new Exception("Only JPG, PNG, and PDF allowed");

            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = folder
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            updateCallback(uploadResult.SecureUrl.ToString(), uploadResult.PublicId);
        }

        // GET All
        public async Task<IEnumerable<CustomerDocumentReadDTO>> GetAllAsync()
        {
            var data = await _repository.GetAllAsync();
            return data.Select(x => new CustomerDocumentReadDTO
            {
                DocumentId = x.DocumentId,
                UserId = x.UserId,
                AgeProofPath = x.AgeProofPath,
                AddressProofPath = x.AddressProofPath,
                DLImagePath = x.DLImagePath,
                CreatedAt = x.CreatedAt,
                Status = x.Status.ToString()
            });
        }

        // GET by User
        public async Task<CustomerDocumentReadDTO> GetByUserIdAsync(int userId)
        {
            var d = await _repository.GetByUserIdAsync(userId);
            if (d == null) return null;

            return new CustomerDocumentReadDTO
            {
                DocumentId = d.DocumentId,
                UserId = d.UserId,
                AgeProofPath = d.AgeProofPath,
                AddressProofPath = d.AddressProofPath,
                DLImagePath = d.DLImagePath,
                CreatedAt = d.CreatedAt,
                Status = d.Status.ToString()
            };
        }

        // DELETE a specific document field
        public async Task UpdateDocumentFieldToNullAndDeleteFileAsync(int userId, string fieldName)
        {
            var d = await _repository.GetByUserIdRawAsync(userId);
            if (d == null) throw new ArgumentException("Documents not found");

            string? publicId = fieldName.ToLower() switch
            {
                "ageproof" => d.AgeProofPublicId,
                "addressproof" => d.AddressProofPublicId,
                "dlimage" => d.DLImagePublicId,
                _ => throw new ArgumentException("Invalid field name")
            };

            // Delete from Cloudinary
            if (!string.IsNullOrEmpty(publicId))
                await _cloudinary.DestroyAsync(new DeletionParams(publicId));

            // Null field in DB
            await _repository.UpdateDocumentFieldToNullAsync(userId, fieldName);
        }

        //Add documnet
        public async Task<bool> HasUserUploadedAnyDocumentsAsync(int userId)
        {
            return await _repository.HasUserUploadedAnyDocumentsAsync(userId);
        }

    }
}