using EZRide_Project.DTO;
using EZRide_Project.Repositories;
using NuGet.Protocol.Core.Types;

namespace EZRide_Project.Services
{
    public class VehicleDetailsService : IVehicleDetailsService
    {
        private readonly IVehicleDetailsRepository _vehicleRepo;

        public VehicleDetailsService(IVehicleDetailsRepository vehicleRepo)
        {
            _vehicleRepo = vehicleRepo;
        }
        //get all data of the vehicle by id
        public VehicleDetailsDto GetVehicleDetails(int vehicleId)
        {
            return _vehicleRepo.GetVehicleDetailsById(vehicleId);
        }

        //get all data of the vehicle
        public List<VehicleDetailsDto> GetAllVehicleDetails()
        {
            return _vehicleRepo.GetAllVehicleDetails();
        }
    }

}
