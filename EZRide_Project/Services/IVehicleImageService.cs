using EZRide_Project.DTO;
using EZRide_Project.Model;

namespace EZRide_Project.Services
{
    public interface IVehicleImageService
    {
        //addd image
        Task<ApiResponseModel> UploadVehicleImageAsync(VehicleImageDTO dto);
        //update image
        Task<ApiResponseModel> UpdateVehicleImageAsync(VehicleImageUpdateDTO dto);
        //delete Image
        Task<ApiResponseModel> DeleteVehicleImageAsync(int vehicleImageId);
        //get all image 
        Task<List<VehicleImageResponseDTO>> GetImagesByVehicleIdAsync(int vehicleId);

    }
}
