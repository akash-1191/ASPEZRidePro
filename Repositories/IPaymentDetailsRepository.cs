using EZRide_Project.DTO;

namespace EZRide_Project.Repositories
{
    public interface IPaymentDetailsRepository
    {
        Task<List<UserBookingPaymentDetailsDto>> GetUserBookingPaymentsAsync(int userId);
    }
}
