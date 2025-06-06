using EZRide_Project.DTO;
using EZRide_Project.Model.Entities;
using EZRide_Project.Repositories;

namespace EZRide_Project.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _repository;

        public ContactService(IContactRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> AddContactAsync(ContactDTO contactDto)
        {


            var contact = new Contact
            {
               
                Subject = contactDto.Subject,
                Message = contactDto.Message,
                Email = contactDto.Email,
                Phone = contactDto.Phone,
                Status = Enum.TryParse<Contact.ContactStatus>(contactDto.Status, out var status)
                            ? status
                            : Contact.ContactStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _repository.AddContactAsync(contact);
            return result != null;
        }


        public async Task<List<ContactDTO>> GetAllContactsAsync()
        {
            var contacts = await _repository.GetAllContactsAsync();

            return contacts.Select(c => new ContactDTO
            {
                Subject = c.Subject,
                Message = c.Message,
                Email = c.Email,
                Phone = c.Phone,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt
            }).ToList();
        }
    }
}
