using EZRide_Project.Data;
using EZRide_Project.DTO.Driver_DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static EZRide_Project.Model.Entities.DriverBookingHistory;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverDashboardController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public DriverDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }



        [Authorize(Roles = "Driver")]
        [HttpGet("DriverTripsDetails")]
        public async Task<IActionResult> GetDriverTrips()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { success = false, message = "Invalid token" });
            }

            int userId = int.Parse(userIdClaim);

            // Get Driver by UserId
            var driver = await _context.Drivers
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (driver == null)
            {
                return Forbid("User is not a driver");
            }

            // Fetching trips related to the driver
            var trips = await _context.DriverBookingHistories
                .Where(dbh => dbh.DriverId == driver.DriverId)
                .Include(dbh => dbh.Booking)
                    .ThenInclude(b => b.User)  // Include Customer details
                .Include(dbh => dbh.Vehicle)  // Include Vehicle details
                .Select(dbh => new OngoingTripDTO
                {
                    DriverFullName = driver.User.Firstname + " " + driver.User.Lastname,
                    DriverPhone = driver.User.Phone,

                    VehicleType = dbh.Vehicle.Vehicletype.ToString(),
                    CarName = dbh.Vehicle.CarName,
                    BikeName = dbh.Vehicle.BikeName,
                    RegistrationNo = dbh.Vehicle.RegistrationNo,
                    DriverId = driver.DriverId,
                    DriverEmail = driver.User.Email,

                    CustomerFullName = dbh.Booking.User.Firstname + " " + dbh.Booking.User.Lastname,
                    CustomerPhone = dbh.Booking.User.Phone,
                    CustomerCity = dbh.Booking.User.City,
                    UesrEmail=dbh.Booking.User.Email,
                    Status = dbh.Status.ToString(),
                    DriverBookingId = dbh.DriverBookingId,
                    StartTime = dbh.Booking.StartTime,
                    EndTime = dbh.Booking.EndTime,
                    DriverAvailabiliStatus = driver.AvailabilityStatus.ToString()
                })
                .ToListAsync();

            if (!trips.Any())
            {
                return NotFound(new { success = false, message = "No trips found for this driver" });
            }

            return Ok(new { success = true, data = trips });
        }



        [Authorize(Roles = "Driver")]
        [HttpPost("UpdateDriverTripStatus")]
        public async Task<IActionResult> UpdateDriverTripStatus(int driverBookingId)
        {
            // Fetch DriverBookingHistory using DriverBookingId
            var driverBooking = await _context.DriverBookingHistories
                .FirstOrDefaultAsync(dbh => dbh.DriverBookingId == driverBookingId);

            if (driverBooking == null)
            {
                return NotFound(new { success = false, message = "Driver booking not found" });
            }

            // Check if the current status is not "InProgress" and then update it
            if (driverBooking.Status != AssignmentStatus.InProgress)
            {
                driverBooking.Status = AssignmentStatus.InProgress;
                driverBooking.CreatedAt = DateTime.Now; // Update timestamp if needed
            }

            try
            {
                // Save changes in the database
                _context.DriverBookingHistories.Update(driverBooking);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Trip status updated to 'InProgress'" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error while updating status", error = ex.Message });
            }
        }


        //upadte complete 
        [Authorize(Roles = "Driver")]
        [HttpPost("UpdateDriverTripStatusComplete")]
        public async Task<IActionResult> UpdateDriverTripStatusComplete(int driverBookingId)
        {
            // Fetch DriverBookingHistory using DriverBookingId
            var driverBooking = await _context.DriverBookingHistories
                .FirstOrDefaultAsync(dbh => dbh.DriverBookingId == driverBookingId);

            if (driverBooking == null)
            {
                return NotFound(new { success = false, message = "Driver booking not found" });
            }

            // Check if the current status is not "Completed" and then update it
            if (driverBooking.Status != AssignmentStatus.Completed)
            {
                driverBooking.Status = AssignmentStatus.Completed;
                driverBooking.CreatedAt = DateTime.Now; // Update timestamp if needed
            }
            try
            {
                
                _context.DriverBookingHistories.Update(driverBooking);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Trip status updated to 'Completed'" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error while updating status", error = ex.Message });
            }
        }


    }
}
