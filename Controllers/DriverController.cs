using System.Security.Claims;
using EZRide_Project.Data;
using EZRide_Project.DTO;
using EZRide_Project.DTO.Driver_DTO;
using EZRide_Project.Model.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static EZRide_Project.Model.Entities.Driver;

namespace EZRide_Project.Controllers
{
    [Route("api/")]
    [ApiController]
    public class DriverController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public DriverController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= ADD DRIVER =================
        [HttpPost("addUpdateDriverExprience")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> UpsertDriver([FromBody] UpsertDriverDto dto)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
                return Unauthorized("Invalid token");

            int userId = int.Parse(userIdClaim.Value);

            var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.UserId == userId);

            // ========== INSERT ==========
            if (driver == null)
            {
                driver = new Driver
                {
                    UserId = userId,
                    ExperienceYears = dto.ExperienceYears,
                    AvailabilityStatus = Driver.AvailabiliStatus.Available,
                    Status = Driver.DriverStatus.Active,
                    VehicleTypes = dto.VehicleTypes,
                    CreatedAt = DateTime.Now
                };

                _context.Drivers.Add(driver);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Driver profile created successfully" });
            }

            // ========== UPDATE ==========
            driver.ExperienceYears = dto.ExperienceYears;

            if (!string.IsNullOrEmpty(dto.AvailabilityStatus))
            {
                driver.AvailabilityStatus =
                    Enum.Parse<Driver.AvailabiliStatus>(dto.AvailabilityStatus, true);
            }
            driver.VehicleTypes = dto.VehicleTypes;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Driver profile updated successfully" });
        }

        // ================= GET SELF DRIVER =================
        [HttpGet("getDriverExprience")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> GetMyDriverProfile()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
                return Unauthorized("Invalid Token");

            int userId = int.Parse(userIdClaim.Value);

            var driver = await _context.Drivers
                .Where(d => d.UserId == userId)
                .Select(d => new DriverResponseDto
                {
                    DriverId = d.DriverId,
                    UserId = d.UserId,
                    ExperienceYears = d.ExperienceYears,
                    AvailabilityStatus = d.AvailabilityStatus.ToString(),
                    VehicleType = d.VehicleTypes,
                    Status = d.Status.ToString(),
                    CreatedAt = d.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (driver == null)
                return NotFound("Driver profile not found");

            return Ok(driver);
        }

        // API to get available drivers based on vehicle type and date
        //[HttpGet("getAllDriversWithReviews")]
        //public async Task<IActionResult> GetAllDriversWithReviews()
        //{
        //    // Fetching all drivers with their details and reviews
        //    var drivers = await _context.Drivers
        //        .Where(d => d.User.Status == Model.Entities.User.UserStatus.Active)  // Active drivers by usderid only
        //        .Select(d => new
        //        {
        //            DriverId = d.DriverId,
        //            Firstname = d.User.Firstname,
        //            Lastname = d.User.Lastname,
        //            Phone = d.User.Phone,
        //            profileimage = d.User.Image,
        //            VehicleType = d.VehicleTypes.ToString(),
        //            ExperienceYears = d.ExperienceYears,
        //            Status = d.Status.ToString(),
        //            AvailabilityStatus = d.AvailabilityStatus.ToString(),
        //            Reviews = _context.DriverReviews
        //                .Where(dr => dr.DriverId == d.DriverId)
        //                .Select(dr => new
        //                {
        //                    Rating = dr.Rating,
        //                    Feedback = dr.Feedback,
        //                    CreatedAt = dr.CreatedAt
        //                }).ToList()
        //        }).ToListAsync();

        //    // If no drivers found
        //    if (!drivers.Any())
        //    {
        //        return NotFound("No drivers available.");
        //    }

        //    return Ok(drivers);  // Return list of drivers with reviews
        //}





        [HttpGet("available-drivers")]
        public async Task<IActionResult> GetAvailableDrivers(
    DateTime startTime,
    DateTime endTime,
    string vehicleType // "Car" or "Bike"
)
        {
            // STEP 1: Busy drivers (already booked in this time range)
            var busyDriverIds = await _context.Bookings
                .Where(b =>
                    b.DriverId != null &&
                    (b.Status == Booking.BookingStatus.Confirmed ||
                     b.Status == Booking.BookingStatus.InProgress) &&
                    b.StartTime < endTime &&
                    b.EndTime.AddHours(2) > startTime
                )
                .Select(b => b.DriverId.Value)
                .Distinct()
                .ToListAsync();

            // STEP 2: Base driver filter
            var driversQuery = _context.Drivers
                .Where(d =>
                    d.Status == Driver.DriverStatus.Active &&                 // Driver Active
                    d.User.Status == Model.Entities.User.UserStatus.Active &&                // User Active
                    !busyDriverIds.Contains(d.DriverId)                       
                );

            // STEP 3: Vehicle type filter
            if (vehicleType == "Car")
            {
                driversQuery = driversQuery.Where(d =>
                    d.VehicleTypes == Driver.VehicleType.FourWheeler ||
                    d.VehicleTypes == Driver.VehicleType.Both
                );
            }
            else if (vehicleType == "Bike")
            {
                driversQuery = driversQuery.Where(d =>
                    d.VehicleTypes == Driver.VehicleType.TwoWheeler ||
                    d.VehicleTypes == Driver.VehicleType.Both
                );
            }

            // STEP 4: Final response with reviews
            var availableDrivers = await driversQuery
                .Select(d => new
                {
                    DriverId = d.DriverId,
                    Firstname = d.User.Firstname,
                    Lastname = d.User.Lastname,
                    Phone = d.User.Phone,
                    ProfileImage = d.User.Image,
                    ExperienceYears = d.ExperienceYears,
                    VehicleType = d.VehicleTypes.ToString(),
                    AvailabilityStatus = d.AvailabilityStatus.ToString(),
                    Reviews = d.DriverReviews.Select(r => new
                    {
                        r.Rating,
                        r.Feedback,
                        r.CreatedAt
                    }).ToList()
                })
                .ToListAsync();

            return Ok(availableDrivers);
        }









        [HttpPost("create-driver-booking")]
        public async Task<IActionResult> CreateDriverBooking(DriverBooking driverBookingDTO)
        {
            if (driverBookingDTO == null)
            {
                return BadRequest("Invalid driver booking data.");
            }

            // Verify if the BookingId exists
            var booking = await _context.Bookings.FindAsync(driverBookingDTO.BookingId);
            if (booking == null)
            {
                return BadRequest("Booking ID does not exist.");
            }

            // Proceed if BookingId is valid
            var driverBooking = new DriverBookingHistory
            {
                BookingId = driverBookingDTO.BookingId,
                DriverId = driverBookingDTO.DriverId,
                VehicleId = driverBookingDTO.VehicleId,
                AssignTime = driverBookingDTO.AssignTime,
                StartTime = driverBookingDTO.StartTime,
                EndTime = driverBookingDTO.EndTime,
                Status = DriverBookingHistory.AssignmentStatus.Assigned,  // Default status as 'Assigned'
                CreatedAt = DateTime.Now
            };

            _context.DriverBookingHistories.Add(driverBooking);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Driver booking created successfully.", data = driverBooking });
        }



        [HttpGet("check-driver-availability/{vehicleId}/{bookingId}")]
        public async Task<IActionResult> CheckDriverAvailability(int vehicleId, int? bookingId)
        {
            // Checking driver availability based on vehicleId
            var availableDrivers = await _context.Drivers
                .Where(d => d.VehicleTypes == (vehicleId == 1 ? Driver.VehicleType.FourWheeler : Driver.VehicleType.TwoWheeler)
                            && d.AvailabilityStatus == Driver.AvailabiliStatus.Available
                            && d.Status == Driver.DriverStatus.Active)
                .ToListAsync();

            // If bookingId is provided, check if any driver is already assigned to that booking
            if (bookingId.HasValue)
            {
                var booking = await _context.Bookings
                    .Include(b => b.Driver)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId.Value);

                if (booking != null && booking.Driver != null)
                {
                    return Ok(new { available = false, message = "Driver already assigned to this booking." });
                }
            }

            // If drivers are available
            if (availableDrivers.Any())
            {
                return Ok(new { available = true, message = "Drivers are available for this vehicle.", drivers = availableDrivers });
            }

            return Ok(new { available = false, message = "No drivers available for this vehicle." });
        }


        [Authorize(Roles = "Driver")]
        [HttpGet("driver-dashboard")]
        public async Task<IActionResult> GetDriverDashboard()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("UserId not found in token");
            }

            int userId = int.Parse(userIdClaim);


            var driver = await _context.Drivers
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (driver == null)
                return NotFound("Driver not found");

            int driverId = driver.DriverId;
            var today = DateTime.Today;

            // Total Trips
            var totalTrips = await _context.Bookings
                .CountAsync(b => b.DriverId == driverId &&
                                 b.Status == Booking.BookingStatus.Completed);

            // Today's Trips
            var todaysTrips = await _context.Bookings
                .CountAsync(b => b.DriverId == driverId &&
                                 b.StartTime.Date == today);

            // Ongoing Trip
            var ongoingTrip = await _context.Bookings
                .Where(b => b.DriverId == driverId &&
                            b.Status == Booking.BookingStatus.InProgress)
                .Select(b => new
                {
                    b.BookingId,
                    b.StartTime,
                    b.EndTime,
                    CustomerName = b.User.Firstname + " " + b.User.Lastname,
                    VehicleType = b.Vehicle.Vehicletype.ToString()
                })
                .FirstOrDefaultAsync();

            // Total Earnings
            var totalEarnings = await _context.Bookings
                .Where(b => b.DriverId == driverId &&
                            b.Status == Booking.BookingStatus.Completed)
                .SumAsync(b => (decimal?)b.TotalAmount) ?? 0;

            var response = new DriverDashboardDto
            {
                DriverName = driver.User.Firstname + " " + driver.User.Lastname,
                TodayDate = DateTime.Now,
                TotalTrips = totalTrips,
                TodaysTrips = todaysTrips,
                OngoingTrip = ongoingTrip,
                TotalEarnings = totalEarnings
            };

            return Ok(response);
        }



    }
}



