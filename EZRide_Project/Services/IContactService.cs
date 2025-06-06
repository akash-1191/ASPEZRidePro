using EZRide_Project.DTO;

namespace EZRide_Project.Services
{
    public interface IContactService
    {
        Task<bool> AddContactAsync(ContactDTO contactDto);
        Task<List<ContactDTO>> GetAllContactsAsync();
    }
}
