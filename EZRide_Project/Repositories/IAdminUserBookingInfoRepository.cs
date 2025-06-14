using EZRide_Project.DTO;

namespace EZRide_Project.Repositories
{
    public interface IAdminUserBookingInfoRepository
    {
        Task<List<AdminUserBookingInfoDto>> GetUserBookingInfoAsync();
    }
}
