using EZRide_Project.DTO;
using EZRide_Project.Model;

namespace EZRide_Project.Services
{
    public interface ICustomerDocumentService
    {
        Task AddAsync(CustomerDocumentCreateDTO dto);
        Task<IEnumerable<CustomerDocumentReadDTO>> GetAllAsync();
        Task<CustomerDocumentReadDTO> GetByUserIdAsync(int userId);
       
        
        Task<bool> HasUserUploadedAnyDocumentsAsync(int userId);

        Task UpdateDocumentFieldToNullAndDeleteFileAsync(int userId, string fieldName);
    }
}
