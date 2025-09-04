using EZRide_Project.Model;

namespace EZRide_Project.Services
{
    public interface IPaymentDetailsService
    {
        Task<ApiResponseModel> GetUserBookingPaymentsAsync(int userId);
    }
}
