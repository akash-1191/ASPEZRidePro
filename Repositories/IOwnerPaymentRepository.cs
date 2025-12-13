using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Model.Entities;

namespace EZRide_Project.Repositories
{
    public interface IOwnerPaymentRepository
    {

        Task<OwnerPayment> AddPaymentAsync(OwnerPayment payment);
        Task<bool> UpdateVehicleAvailabilityStatusAsync(int vehicleId, int ownerId);

        Task<List<OwnerPayment>> GetOwnerPaymentsAsync(int ownerId);
        Task<OwnerVehicleAvailability> GetAvailabilityAtDateAsync(int vehicleId, DateTime date);
        Task<OwnerVehicleAvailability> GetLatestAvailabilityAsync(int vehicleId);
        Task<Vehicle> GetVehicleByIdAsync(int vehicleId);
    }
}
