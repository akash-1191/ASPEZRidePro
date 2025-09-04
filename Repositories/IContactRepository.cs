using EZRide_Project.Model.Entities;

namespace EZRide_Project.Repositories
{
    public interface IContactRepository
    {
        Task<Contact> AddContactAsync(Contact contact);

        Task<List<Contact>> GetAllContactsAsync();
    }
}
