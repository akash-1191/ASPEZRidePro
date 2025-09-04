using EZRide_Project.DTO;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace EZRide_Project.Controllers
{
    [Route("api/")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        //add vehicle data
        [HttpPost("addVehicle")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddVehicle([FromBody] VehicleCreateDTO dto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null)
                return Unauthorized("UserId claim not found in token.");

            int userId = int.Parse(userIdClaim.Value);

            var result = await _vehicleService.AddVehicleAsync(dto, userId);
            return StatusCode(result.StatusCode, result);
        }


        //getall vehicle data

        [HttpGet("getAllVehicles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllVehicles()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();

            if (vehicles == null || !vehicles.Any())
                return NotFound("No vehicles found.");

            return Ok(vehicles);
        }

        //updtae data of the vehicle 
        [HttpPut("updateVehicle")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateVehicle([FromBody] VehicleCreateDTO dto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null)
                return Unauthorized("UserId claim not found in token.");

            int userId = int.Parse(userIdClaim.Value);

            var result = await _vehicleService.UpdateVehicleAsync(dto, userId);
            return StatusCode(result.StatusCode, result);
        }

        //Delete vehicle data
        [HttpDelete("deleteVehicle/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var result = await _vehicleService.DeleteVehicleAsync(id);
            return StatusCode(result.StatusCode, result);
        }



    }
}
