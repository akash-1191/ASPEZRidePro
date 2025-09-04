using EZRide_Project.Model.Entities;

namespace EZRide_Project.Repositories
{
    public interface ICustomerDocumentRepository
    {
        Task AddAsync(CustomerDocument document);
        Task<IEnumerable<CustomerDocument>> GetAllAsync();
        Task<CustomerDocument> GetByUserIdAsync(int userId);
        Task<bool> ExistsByUserIdAsync(int userId);
        Task<CustomerDocument> GetByIdAsync(int id);
        Task<bool> HasUserUploadedAnyDocumentsAsync(int userId);


        Task UpdateDocumentFieldToNullAsync(int userId, string fieldName);
        Task<CustomerDocument?> GetByUserIdRawAsync(int userId);
        Task UpdateAsync(CustomerDocument document);

    }

}
