using EZRide_Project.Data;
using EZRide_Project.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace EZRide_Project.Repositories
{
    public class CustomerDocumentRepository : ICustomerDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerDocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CustomerDocument document)
        {
            await _context.CustomerDocuments.AddAsync(document);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByUserIdAsync(int userId)
        {
            return await _context.CustomerDocuments.AnyAsync(d => d.UserId == userId);
        }
        public async Task<IEnumerable<CustomerDocument>> GetAllAsync()
        {
            return await _context.CustomerDocuments.ToListAsync();
        }

        public async Task<CustomerDocument> GetByUserIdAsync(int userId)
        {
            return await _context.CustomerDocuments
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task<CustomerDocument> GetByIdAsync(int id)
        {
            return await _context.CustomerDocuments.FirstOrDefaultAsync(d => d.DocumentId == id);
        }

        public async Task<bool> HasUserUploadedAnyDocumentsAsync(int userId)
        {
            var doc = await _context.CustomerDocuments.FirstOrDefaultAsync(x => x.UserId == userId);

            if (doc == null) return false;

            // Agar koi bhi file uploaded hai to return true
            bool allUploaded =
                   !string.IsNullOrEmpty(doc.AgeProofPath) &&
                   !string.IsNullOrEmpty(doc.AddressProofPath) &&
                   !string.IsNullOrEmpty(doc.DLImagePath);

            return allUploaded;
        }


        public async Task UpdateDocumentFieldToNullAsync(int userId, string fieldName)
        {
            var document = await _context.CustomerDocuments.FirstOrDefaultAsync(x => x.UserId == userId);
            if (document == null) return;

            switch (fieldName.ToLower())
            {
                case "ageproof":
                    document.AgeProofPath = null;
                    break;
                case "addressproof":
                    document.AddressProofPath = null;
                    break;
                case "dlimage":
                    document.DLImagePath = null;
                    break;
                default:
                    throw new ArgumentException("Invalid field name.");
            }

            await _context.SaveChangesAsync();
        }

        //frist add and then update
        public async Task<CustomerDocument?> GetByUserIdRawAsync(int userId)
        {
            return await _context.CustomerDocuments.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task UpdateAsync(CustomerDocument document)
        {
            _context.CustomerDocuments.Update(document);
            await _context.SaveChangesAsync();
        }
    }
}



