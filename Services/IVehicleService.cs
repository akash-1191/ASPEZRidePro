using EZRide_Project.DTO;
using EZRide_Project.Model;
using EZRide_Project.Model.Entities;

namespace EZRide_Project.Services
{
    public interface IVehicleService
    {
        Task<ApiResponseModel> AddVehicleAsync(VehicleCreateDTO dto, int userId);
        

        //Get all vehicala list
        Task<List<VehicleCreateDTO>> GetAllVehiclesAsync(int adminId);

        //updatedata of the vehicle 
        Task<ApiResponseModel> UpdateVehicleAsync(VehicleCreateDTO dto, int userId);


        //delete data
        Task<ApiResponseModel> DeleteVehicleAsync(int vehicleId);
    }
}
