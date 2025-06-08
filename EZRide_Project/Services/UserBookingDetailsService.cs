using EZRide_Project.DTO;
using EZRide_Project.Repositories;

namespace EZRide_Project.Services
{
   
        public class UserBookingDetailsService : IUserBookingDetailsService
        {
            private readonly IUserBookingDetailsRepository _repository;

            public UserBookingDetailsService(IUserBookingDetailsRepository repository)
            {
                _repository = repository;
            }
        public async Task<List<UserBookingDetailsDto>> GetAllUsersBookingDetailsAsync()
        {
            return await _repository.GetAllUsersBookingDetailsAsync();
        }
    }
    
}
