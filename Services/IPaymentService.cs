using EZRide_Project.DTO;

namespace EZRide_Project.Services
{
    public interface IPaymentService
    {
        Task<bool> VerifyAndSavePaymentAsync(RazorpayVerificationDto dto);


    }
    
}
