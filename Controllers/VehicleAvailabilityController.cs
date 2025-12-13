using System;
using EZRide_Project.Data;
using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleAvailabilityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VehicleAvailabilityController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("availabilityOwnerVehicle/{vehicleId}")]
        public async Task<IActionResult> GetVehicleAvailability(int vehicleId)
        {
            // 1️⃣ Vehicle fetch
            var vehicle = await _context.Vehicles
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);

            if (vehicle == null)
                return NotFound("Vehicle not found");

            // 2️⃣ Vehicle owner fetch with Role
            var owner = await _context.Users
                .Include(u => u.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == vehicle.UserId);

            if (owner == null || owner.Role == null)
                return BadRequest("Vehicle owner or role not found");

            // 3️⃣ ADMIN vehicle → unlimited
            if (owner.Role.RoleName == Role.Rolename.Admin)
            {
                return Ok(new VehicleAvailabilityDto
                {
                    VehicleId = vehicleId,
                    OwnershipType = "ADMIN",
                    IsUnlimited = true,
                    AvailableFrom = null,
                    AvailableTo = null,
                    Status = "Active"
                });
            }

            // 4️⃣ OWNER vehicle → check OwnerVehicleAvailability
            var availability = await _context.OwnerVehicleAvailabilities
                .AsNoTracking()
                .Where(a => a.VehicleId == vehicleId &&
                            a.OwnerId == owner.UserId)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();

            // No availability record
            if (availability == null)
            {
                return Ok(new VehicleAvailabilityDto
                {
                    VehicleId = vehicleId,
                    OwnershipType = "OWNER",
                    IsUnlimited = false,
                    AvailableFrom = null,
                    AvailableTo = null,
                    Status = "NotAvailable"
                });
            }

            // Expired availability
            if (availability.Status == OwnerVehicleAvailability.AvailabilityStatus.Expired ||
                availability.EffectiveTo < DateTime.UtcNow)
            {
                return Ok(new VehicleAvailabilityDto
                {
                    VehicleId = vehicleId,
                    OwnershipType = "OWNER",
                    IsUnlimited = false,
                    AvailableFrom = availability.EffectiveFrom,
                    AvailableTo = availability.EffectiveTo,
                    Status = "Expired"
                });
            }

            // Active availability
            return Ok(new VehicleAvailabilityDto
            {
                VehicleId = vehicleId,
                OwnershipType = "OWNER",
                IsUnlimited = false,
                AvailableFrom = availability.EffectiveFrom,
                AvailableTo = availability.EffectiveTo,
                Status = "Active"
            });
        }
    

    }
}
