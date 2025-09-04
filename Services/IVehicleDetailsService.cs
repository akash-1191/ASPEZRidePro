using EZRide_Project.DTO;

namespace EZRide_Project.Services
{
    public interface IVehicleDetailsService
    {
        //get all data of the vehicle by vehicle id
        VehicleDetailsDto GetVehicleDetails(int vehicleId);

        //get all data of the vehicle
        List<VehicleDetailsDto> GetAllVehicleDetails();
    }
}
