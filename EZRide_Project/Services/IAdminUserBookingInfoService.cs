using EZRide_Project.DTO;

namespace EZRide_Project.Services
{

    public interface IAdminUserBookingInfoService
    {
        Task<List<AdminUserBookingInfoDto>> GetUserBookingInfoAsync();
    }
}
