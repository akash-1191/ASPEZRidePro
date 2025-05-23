using EZRide_Project.DTO;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EZRide_Project.Controllers
{
    [Route("api/")]
    [ApiController]
    public class VehiclesDetailController : ControllerBase
    {
        private readonly IVehicleDetailsService _vehicleDetailsService;

        public VehiclesDetailController(IVehicleDetailsService vehicleDetailsService)
        {
            _vehicleDetailsService = vehicleDetailsService;
        }
        //get all data of the vehicle by id
        [HttpGet("GetAllVehicleById/{vehicleId}")]
        

        public IActionResult GetVehicleDetails(int vehicleId)
        {
            var result = _vehicleDetailsService.GetVehicleDetails(vehicleId);
            if (result == null)
                return NotFound("Vehicle not found");

            return Ok(result);
        }

        //get all data of the vehicle
        [HttpGet]
        [Route("GetAllVehicle")]
        public ActionResult<List<VehicleDetailsDto>> GetAll()
        {
            var vehicles = _vehicleDetailsService.GetAllVehicleDetails();
            if (vehicles == null || vehicles.Count == 0)
                return NotFound("No vehicles found");

            return Ok(vehicles);
        }
    }
}
