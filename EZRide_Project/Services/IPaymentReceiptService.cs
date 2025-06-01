using EZRide_Project.DTO;

namespace EZRide_Project.Services
{
    public interface IPaymentReceiptService
    {
        Task<PaymentReceiptDto?> GetPaymentReceiptAsync(int userId, int bookingId);
        byte[] GeneratePdf(PaymentReceiptDto dto);
        Task SendReceiptEmailAsync(string toEmail, int userId, int bookingId);
    }
}
