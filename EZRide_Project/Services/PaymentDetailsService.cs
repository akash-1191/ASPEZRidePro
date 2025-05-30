using EZRide_Project.Helpers;
using EZRide_Project.Model;
using EZRide_Project.Repositories;

namespace EZRide_Project.Services
{
    public class PaymentDetailsService : IPaymentDetailsService
    {
        private readonly IPaymentDetailsRepository _paymentDetailsRepository;

        public PaymentDetailsService(IPaymentDetailsRepository paymentDetailsRepository)
        {
            _paymentDetailsRepository = paymentDetailsRepository;
        }

        public async Task<ApiResponseModel> GetUserBookingPaymentsAsync(int userId)
        {
            var data = await _paymentDetailsRepository.GetUserBookingPaymentsAsync(userId);

            if (data == null || !data.Any())
            {
                return ApiResponseHelper.NotFound("No bookings found for this user.");
            }

            return ApiResponseHelper.Success("User bookings retrieved successfully.", data);
        }
    }
}

