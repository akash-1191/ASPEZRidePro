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

        public async Task<IEnumerable<CustomerDocument>> GetAllAsync()
        {
            return await _context.CustomerDocuments.ToListAsync();
        }

        public async Task<CustomerDocument> GetByIdAsync(int id)
        {
            return await _context.CustomerDocuments.FindAsync(id);
        }

        public async Task DeleteAsync(int id)
        {
            var doc = await _context.CustomerDocuments.FindAsync(id);
            if (doc != null)
            {
                _context.CustomerDocuments.Remove(doc);
                await _context.SaveChangesAsync();
            }
        }
    }
}


