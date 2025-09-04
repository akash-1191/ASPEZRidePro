using EZRide_Project.DTO;

namespace EZRide_Project.Repositories
{
    public interface IPaymentReceiptRepository
    {
        Task<PaymentReceiptDto?> GetPaymentReceiptByUserIdAndBookingIdAsync(int userId, int bookingId);

        Task<PaymentReceiptDto?> GetPaymentReceiptByUserIdAsync(int userId, int bookingId);

    }
}
