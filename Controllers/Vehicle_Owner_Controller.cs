using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Vehicle_Owner_Controller : ControllerBase
    {
        private readonly IOwnerVehicleService _ownerVehicleService;

        public Vehicle_Owner_Controller(IOwnerVehicleService ownerVehicleService)
        {
            _ownerVehicleService = ownerVehicleService;
        }

        // Get vehicles only for logged-in owner
        [Authorize(Roles = "OwnerVehicle,Admin")]
        [HttpGet("get-owner-vehicles")]
        public async Task<IActionResult> GetOwnerVehicles()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
                return Unauthorized("Invalid token, UserId not found.");

            int ownerId = int.Parse(userIdClaim.Value);

            var result = await _ownerVehicleService.GetOwnerVehiclesAsync(ownerId);

            return Ok(result);
        }


        // UPDATE Vehicle by Owner
        [HttpPut("update-owner-vehicle")]
        [Authorize(Roles = "OwnerVehicle")]
        public async Task<IActionResult> UpdateOwnerVehicle([FromBody] VehicleDTO dto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null)
                return Unauthorized("UserId claim not found in token.");

            int ownerId = int.Parse(userIdClaim.Value);

            var result = await _ownerVehicleService.UpdateOwnerVehicleAsync(dto, ownerId);
            return StatusCode(result.StatusCode, result);
        }


        //delete vehicle By owner
        [HttpDelete("delete-owner-vehicle/{vehicleId}")]
        [Authorize(Roles = "OwnerVehicle")]
        public async Task<IActionResult> DeleteOwnerVehicle(int vehicleId)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null)
                return Unauthorized("UserId claim not found.");

            int ownerId = int.Parse(userIdClaim.Value);

            var result = await _ownerVehicleService.DeleteOwnerVehicleAsync(vehicleId, ownerId);
            return StatusCode(result.StatusCode, result);
        }


        //add owner vehile avalibilityes days
        [HttpPost("addAvalibilityDays")]
        [Authorize(Roles = "OwnerVehicle")]
        public async Task<IActionResult> AddAvailability([FromBody] AddAvailabilityDto dto)
        {
            var ownerIdClaim = User.FindFirst("UserId");
            if (ownerIdClaim == null)
                return Unauthorized("Invalid token");

            int ownerId = int.Parse(ownerIdClaim.Value);

            var result = await _ownerVehicleService.AddAvailabilityAsync(dto, ownerId);

            if (result.StartsWith("Unauthorized"))
                return Unauthorized(result);

            return Ok(result);
        }



    }



}