using EZRide_Project.Data;
using EZRide_Project.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Repositories
{
    public class OwnerDocumentRepository : IOwnerDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public OwnerDocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<OwnerDocument>> GetOwnerDocumentsAsync(int ownerId)
        {
            return await _context.OwnerDocuments
                .Where(d => d.OwnerId == ownerId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<OwnerDocument?> GetByIdAsync(int documentId)
        {
            return await _context.OwnerDocuments.FirstOrDefaultAsync(d => d.DocumentId == documentId);
        }

        public async Task<OwnerDocument?> GetOwnerDocumentByType(int ownerId, string documentType)
        {
            var type = Enum.Parse<OwnerDocument.documentType>(documentType, true);
            return await _context.OwnerDocuments
                .FirstOrDefaultAsync(d => d.OwnerId == ownerId && d.DocumentType == type);
        }

        public async Task AddAsync(OwnerDocument document)
        {
            await _context.OwnerDocuments.AddAsync(document);
        }

        public async Task UpdateAsync(OwnerDocument document)
        {
            _context.OwnerDocuments.Update(document);
        }

        public void Delete(OwnerDocument document)
        {
            _context.OwnerDocuments.Remove(document);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }

}
