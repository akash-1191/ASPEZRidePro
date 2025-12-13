using System.Security.Claims;
using EZRide_Project.Data;
using EZRide_Project.Model.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "OwnerVehicle,Admin")]
    public class VehicleReRentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VehicleReRentController> _logger;

        public VehicleReRentController(ApplicationDbContext context, ILogger<VehicleReRentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("Re-Rent/{vehicleId}")]
        public async Task<IActionResult> ResetVehicleForReRent(int vehicleId)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null)
                    return Unauthorized(new { message = "Invalid token" });

                int currentUserId = int.Parse(userIdClaim.Value);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                var vehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);

                if (vehicle == null)
                {
                    return NotFound(new { message = $"Vehicle with ID {vehicleId} not found" });
                }

                if (userRole != "Admin" && vehicle.UserId != currentUserId)
                {
                    return Unauthorized(new { message = "You are not authorized to reset this vehicle" });
                }

                if (vehicle.Availability == Vehicle.AvailabilityStatus.Booked)
                {
                    return BadRequest(new
                    {
                        message = "Vehicle is currently booked. Cannot reset during active booking.",
                        currentStatus = vehicle.Availability.ToString()
                    });
                }

                if (vehicle.Availability == Vehicle.AvailabilityStatus.Disabled)
                {
                    return BadRequest(new
                    {
                        message = "Vehicle is disabled. Enable it first before resetting.",
                        currentStatus = vehicle.Availability.ToString()
                    });
                }

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    DateTime currentDate = DateTime.Now;

                    vehicle.SecurityDepositAmount = null;
                    vehicle.IsApproved = false;
                    vehicle.RejectReason = null;
                    vehicle.Availability = Vehicle.AvailabilityStatus.Available;
                    vehicle.CreatedAt = currentDate;

                    _context.Vehicles.Update(vehicle);

                    var availability = await _context.OwnerVehicleAvailabilities
                        .FirstOrDefaultAsync(a => a.VehicleId == vehicleId && a.OwnerId == vehicle.UserId);

                    if (availability != null)
                    {
                        availability.AvailableDays = 0;
                        availability.EffectiveFrom = currentDate;
                        availability.EffectiveTo = currentDate;
                        availability.Status = OwnerVehicleAvailability.AvailabilityStatus.Active;
                        availability.vehicleAmountPerDay = 0;
                        availability.CreatedAt = currentDate;

                        _context.OwnerVehicleAvailabilities.Update(availability);
                    }
                    else
                    {
                        var newAvailability = new OwnerVehicleAvailability
                        {
                            VehicleId = vehicleId,
                            OwnerId = vehicle.UserId,
                            AvailableDays = 0,
                            EffectiveFrom = currentDate,
                            EffectiveTo = currentDate,
                            Status = OwnerVehicleAvailability.AvailabilityStatus.Active,
                            vehicleAmountPerDay = 0,
                            CreatedAt = currentDate
                        };

                        await _context.OwnerVehicleAvailabilities.AddAsync(newAvailability);
                    }

                    var pricingRule = await _context.PricingRules
                        .FirstOrDefaultAsync(p => p.VehicleId == vehicleId);

                    if (pricingRule != null)
                    {
                        pricingRule.PricePerKm = null;
                        pricingRule.PricePerHour = null;
                        pricingRule.PricePerDay = null;
                        pricingRule.CreatedAt = currentDate;

                        _context.PricingRules.Update(pricingRule);
                    }
                    else
                    {
                        var newPricingRule = new PricingRule
                        {
                            VehicleId = vehicleId,
                            PricePerKm = null,
                            PricePerHour = null,
                            PricePerDay = null,
                            CreatedAt = currentDate
                        };

                        await _context.PricingRules.AddAsync(newPricingRule);
                    }

                    // UPDATE PAYMENT STATUS TO ReRent
                    var payments = await _context.OwnerPayments
                        .Where(p => p.VehicleId == vehicleId && p.UserId == vehicle.UserId)
                        .ToListAsync();

                    foreach (var payment in payments)
                    {
                        payment.Status = OwnerPayment.PaymentStatus.ReRent;
                    }

                    if (payments.Any())
                    {
                        _context.OwnerPayments.UpdateRange(payments);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(new
                    {
                        success = true,
                        message = "Vehicle successfully reset for re-rent!",
                        vehicleId = vehicleId,
                        resetTime = currentDate,
                        paymentsUpdated = payments.Count
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "Error resetting vehicle",
                        error = ex.Message
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }
    }

}
