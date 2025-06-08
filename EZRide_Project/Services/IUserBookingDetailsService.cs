using EZRide_Project.DTO;

namespace EZRide_Project.Services
{
    public interface IUserBookingDetailsService
    {
        Task<List<UserBookingDetailsDto>> GetAllUsersBookingDetailsAsync();
    }
}
