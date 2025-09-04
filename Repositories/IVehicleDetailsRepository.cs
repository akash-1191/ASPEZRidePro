using EZRide_Project.DTO;

namespace EZRide_Project.Repositories
{
    public interface IVehicleDetailsRepository
    {
       //get by vehicle id
        VehicleDetailsDto GetVehicleDetailsById(int vehicleId);
       //get all data without vehoicle
        List<VehicleDetailsDto> GetAllVehicleDetails();
    }
}
