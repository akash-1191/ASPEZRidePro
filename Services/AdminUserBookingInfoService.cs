using EZRide_Project.DTO;
using EZRide_Project.Repositories;

namespace EZRide_Project.Services
{
    public class AdminUserBookingInfoService : IAdminUserBookingInfoService
    {
        private readonly IAdminUserBookingInfoRepository _repo;

        public AdminUserBookingInfoService(IAdminUserBookingInfoRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<AdminUserBookingInfoDto>> GetUserBookingInfoAsync()
        {
            return await _repo.GetUserBookingInfoAsync();
        }
    }

}
