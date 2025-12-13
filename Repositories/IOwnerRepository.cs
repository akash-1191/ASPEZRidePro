using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Model.Entities;

namespace EZRide_Project.Repositories
{
    public interface IOwnerRepository
    {
        Task<List<OwnerApprovalDTO>> GetPendingOwnersAsync();
        User? GetUserById(int id);
        //IEnumerable<User> GetPendingOwners();
        void UpdateUser(User user);
        Task<List<Vehicle>> GetAllOwnerVehiclesAsync(int ownerId);
        Task<List<User>> GetAllActiveOwnersAsync();

        Task<Vehicle> GetVehicleByIdAsync(int vehicleId);
        Task<bool> SaveChangesAsync();

   
        Task UpdateVehicleAsync(Vehicle vehicle);
        Task<OwnerVehicleAvailability> GetAvailabilityByIdAsync(int availabilityId);
        Task UpdateAvailabilityAsync(OwnerVehicleAvailability availability);

        Task<List<OwnerPaymentInfoDto>> GetOwnerPaymentDataAsync();




    }
}
