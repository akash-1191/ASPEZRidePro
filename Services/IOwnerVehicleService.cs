using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Model;

namespace EZRide_Project.Services
{
    public interface IOwnerVehicleService
    {

        Task<List<VehicleDTO>> GetOwnerVehiclesAsync(int ownerId);
        Task<ApiResponseModel> UpdateOwnerVehicleAsync(VehicleDTO dto, int ownerId);
        Task<ApiResponseModel> DeleteOwnerVehicleAsync(int vehicleId, int ownerId);

        //add owner vehile avalibilityes days
        Task<string> AddAvailabilityAsync(AddAvailabilityDto dto, int ownerId);


    }
}
