using EZRide_Project.DTO;

namespace EZRide_Project.Services
{
    public interface IBookingSummaryService
    {
        BookingSummaryDTO GetTotalBookingsByUserId(int userId);
        Task<VehicleBookingCountDTO> GetBookedVehicleTypeCountAsync();

        int GetAvailableVehicleCount();
        Task<int> GetPendingPaymentCountAsync(int userId);
        Task<RefundInfoDto?> GetLatestRefundAsync(int userId);

    }
}
