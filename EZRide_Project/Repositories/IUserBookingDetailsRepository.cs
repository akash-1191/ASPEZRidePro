using EZRide_Project.DTO;

namespace EZRide_Project.Repositories
{

    public interface IUserBookingDetailsRepository
    {
        Task<List<UserBookingDetailsDto>> GetAllUsersBookingDetailsAsync();
    }
}
