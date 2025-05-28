using EZRide_Project.Model.Entities;

namespace EZRide_Project.Repositories
{
    public interface ICustomerDocumentRepository
    {
        Task AddAsync(CustomerDocument document);
        Task<IEnumerable<CustomerDocument>> GetAllAsync();
        Task<CustomerDocument> GetByIdAsync(int id);
        Task DeleteAsync(int id);
    }

}
