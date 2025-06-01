using EZRide_Project.DTO;

namespace EZRide_Project.Repositories
{
    public interface IBookingSummaryRepository
    {
        int GetTotalBookingsByUserId(int userId);
        Task<VehicleBookingCountDTO> GetBookedVehicleTypeCountAsync();
        int GetAvailableVehicleCount();
        Task<int> GetPendingPaymentCountAsync(int userId);
        Task<RefundInfoDto?> GetLatestRefundAsync(int userId);
    }
}
