using EZRide_Project.DTO;
using EZRide_Project.Model;

namespace EZRide_Project.Services
{
    public interface ICustomerDocumentService
    {
        Task AddAsync(CustomerDocumentCreateDTO dto);
        Task<IEnumerable<CustomerDocumentReadDTO>> GetAllAsync();
        Task<CustomerDocumentReadDTO> GetByIdAsync(int id);
        Task DeleteAsync(int id);
    }
}
