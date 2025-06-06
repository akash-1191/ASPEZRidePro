using EZRide_Project.Data;
using EZRide_Project.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly ApplicationDbContext _context;

        public ContactRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Contact> AddContactAsync(Contact contact)
        {
            contact.CreatedAt = DateTime.UtcNow;
            _context.Set<Contact>().Add(contact);
            await _context.SaveChangesAsync();
            return contact;
        }


        public async Task<List<Contact>> GetAllContactsAsync()
        {
            return await _context.Contacts.OrderByDescending(c => c.CreatedAt).ToListAsync();
        }
    }
}
