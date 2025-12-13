using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Model.Entities;

namespace EZRide_Project.Services
{
    public interface IOwnerPaymentService
    {
        Task<bool> VerifyPaymentAsync(OwnerPaymentVerifyDto dto);
        Task<List<OwnerPaymentDto>> GetOwnerPaymentsAsync(int ownerId);

    }
}
