using EZRide_Project.DTO.Driver_DTO;

namespace EZRide_Project.Services
{
    public interface IDriverPaymentService
    {
        Task<bool> VerifyPaymentAsync(DriverPaymentVerifyDto dto);

    }
}
