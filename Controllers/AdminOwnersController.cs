using EZRide_Project.DTO;
using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Helpers;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Climate;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminOwnersController : ControllerBase
    {
        private readonly IOwnerService _ownerService;

        public AdminOwnersController(IOwnerService ownerService)
        {
            _ownerService = ownerService;
        }

        [HttpGet("GetPendingOwners")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingOwners()
        {
            var owners = await _ownerService.GetPendingOwnersAsync();

            if (owners == null || owners.Count == 0)
                return NotFound(new { message = "No pending owners found." });

            return Ok(new { message = "Success", data = owners });
        }

        //approval vehicle owner
        [HttpPut("approve/{userId}")]
        public IActionResult ApproveOwner(int userId)
        {
            var result = _ownerService.ApproveOwner(userId);
            return StatusCode(result.StatusCode, result);
        }


        //reject Vehicle Owner
        [HttpPut("reject/{userId}")]
        public IActionResult RejectOwner(int userId, [FromBody] RejectOwnerDTO dto)
        {
            var result = _ownerService.RejectOwner(userId, dto.Reason);
            return StatusCode(result.StatusCode, result);
        }


        //get all vehicle added by the owner
        [HttpGet("getOwnerVehicles/{ownerId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOwnerVehicles(int ownerId)
        {
            var vehicles = await _ownerService.GetAllOwnerVehiclesAsync(ownerId);

            if (vehicles == null || !vehicles.Any())
                return NotFound("No vehicles found.");

            return Ok(vehicles);
        }

        //get all vehicle Aproval owner list
        [HttpGet("getAllActiveOwners")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllActiveOwners()
        {
            var owners = await _ownerService.GetAllActiveOwnersAsync();

            if (owners == null || !owners.Any())
                return NotFound("No active owners found.");

            return Ok(owners);
        }


        //add or upadet security amount
        [HttpPost("addOrUpdateSecurityDeposit")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddOrUpdateDeposit([FromBody] VehicleSecurityDepositDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponseHelper.ValidationFailed("Invalid request data."));

            var response = await _ownerService.AddOrUpdateDepositAsync(dto);
            return StatusCode(response.StatusCode, response);
        }



        // APPROVE VEHICLE
        [HttpPut("approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveVehicle([FromBody] VehicleApproveDto dto)
        {
            if (dto == null || dto.VehicleId <= 0)
                return BadRequest("VehicleId is required.");

            var result = await _ownerService.ApproveVehicleAsync(dto.VehicleId);
            return Ok(new
            {
                isSuccess = true,
                message = result
            });
        }

        // REJECT VEHICLE
        [HttpPut("rejectVehicle")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectVehicle([FromBody] VehicleRejectDto dto)
        {
            if (dto == null || dto.VehicleId <= 0)
                return BadRequest("VehicleId is required.");

            if (string.IsNullOrWhiteSpace(dto.RejectReason))
                return BadRequest("Reject reason is required.");

            var result = await _ownerService.RejectVehicleAsync(dto.VehicleId, dto.RejectReason);
            return Ok(new
            {
                isSuccess = true,
                message = result
            });
        }


        //admin sdet price or the per vehicle to pay vehicle owner
        [HttpPut("updatePrice")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> UpdatePrice([FromBody] UpdateAvailabilityPriceDto dto)
        {
            var result = await _ownerService.UpdatePriceAsync(dto.AvailabilityId, dto.vehicleAmountPerDay);
            if (result.StartsWith("Error"))
                return BadRequest(result);

            return Ok(result);
        }

    }
}
