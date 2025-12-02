using EZRide_Project.Model.Entities;

namespace EZRide_Project.Repositories
{
    public interface IOwnerDocumentRepository
    {
        Task<OwnerDocument?> GetByIdAsync(int documentId);
        Task<OwnerDocument?> GetOwnerDocumentByType(int ownerId, string documentType);
        Task<List<OwnerDocument>> GetOwnerDocumentsAsync(int ownerId);

        Task AddAsync(OwnerDocument document);
        Task UpdateAsync(OwnerDocument document);
        void Delete(OwnerDocument document);
        Task<bool> SaveChangesAsync();
    }
}
