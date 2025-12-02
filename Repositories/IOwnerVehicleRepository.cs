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

     

    }
}
