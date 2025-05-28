using EZRide_Project.DTO;
using EZRide_Project.Helpers;
using EZRide_Project.Model.Entities;
using EZRide_Project.Model;
using EZRide_Project.Repositories;

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

        public async Task AddAsync(CustomerDocumentCreateDTO dto)
        {
            // File save path
            var folderPath = Path.Combine(_env.WebRootPath, "Upload_image", "CustomerDocument");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Save files once only
            string ageProofPath = await SaveFileAsync(dto.AgeProof, folderPath);
            string addressProofPath = await SaveFileAsync(dto.AddressProof, folderPath);
            string dlImagePath = null;

            if (dto.DLImage != null)
            {
                dlImagePath = await SaveFileAsync(dto.DLImage, folderPath);
            }

            // Create document entity
            var document = new CustomerDocument
            {
                UserId = dto.UserId,
                AgeProofPath = ageProofPath,
                AddressProofPath = addressProofPath,
                DLImagePath = dlImagePath,
                CreatedAt = DateTime.UtcNow,
                Status = CustomerDocument.DocumentStatus.Active // Default status
            };

            // If DTO status provided, override default
            if (!string.IsNullOrEmpty(dto.Status) &&
                Enum.TryParse(dto.Status, true, out CustomerDocument.DocumentStatus parsedStatus))
            {
                document.Status = parsedStatus;
            }

            // Save to database
            await _repository.AddAsync(document);
        }

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

        public async Task<CustomerDocumentReadDTO> GetByIdAsync(int id)
        {
            var d = await _repository.GetByIdAsync(id);
            if (d == null) return null;

            return new CustomerDocumentReadDTO
            {
                DocumentId = d.DocumentId,
                UserId = d.UserId,
                AgeProofPath = d.AgeProofPath,
                AddressProofPath = d.AddressProofPath,
                DLImagePath = d.DLImagePath,
                Status = d.Status.ToString(),
                CreatedAt = d.CreatedAt
            };
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        private async Task<string> SaveFileAsync(IFormFile file, string folderPath)
        {
            if (file == null || file.Length == 0)
                return null;

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative path for storing in DB
            return Path.Combine("Upload_image", "CustomerDocument", fileName).Replace("\\", "/");
        }

    }
}