using EZRide_Project.Model.Entities;

namespace EZRide_Project.Repositories
{
    public interface IVehicleRepository
    {
        //insertdata of the vehicletable
        void AddVehicle(Vehicle vehicle);
        Task<bool> SaveChangesAsync();

        //Get all vehical list
        Task<List<Vehicle>> GetAllVehiclesAsync(int adminId);

        //update data of the vehicle 
        Task<Vehicle?> GetVehicleByIdAsync(int vehicleId);
        void UpdateVehicle(Vehicle vehicle);

        //delete data of the vehicle 
        Task<bool> DeleteVehicleAsync(int vehicleId);

       

    }
}
