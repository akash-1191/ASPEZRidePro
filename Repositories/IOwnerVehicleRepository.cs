using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Model;
using EZRide_Project.Model.Entities;

namespace EZRide_Project.Repositories
{
    public interface IOwnerVehicleRepository
    {

        Task<List<Vehicle>> GetVehiclesByOwnerAsync(int ownerId);
        Task<Vehicle> GetVehicleByIdAsync(int vehicleId);
        void UpdateVehicle(Vehicle vehicle);
        void DeleteVehicle(Vehicle vehicle);
        Task<bool> SaveChangesAsync();

        //add owner vehile avalibilityes days

        Task<bool> IsVehicleOwnedByUserAsync(int vehicleId, int ownerId);
        Task AddAvailabilityAsync(OwnerVehicleAvailability availability);

        Task<OwnerVehicleAvailability> GetAvailabilityByVehicleAndOwnerAsync(int vehicleId, int ownerId);
        Task UpdateAvailabilityAsync(OwnerVehicleAvailability availability);

    }
}
