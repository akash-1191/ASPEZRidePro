using EZRide_Project.Data;
using EZRide_Project.DTO.Driver_DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminDriverController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public AdminDriverController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("/driversdata")]
        public async Task<IActionResult> GetAllDriversForAdmin()
        {
            var drivers = await _context.Drivers
                .Include(d => d.User)
                .Include(d => d.DriverDocuments)
                .Select(d => new DriverAdminListDto
                {
                    DriverId = d.DriverId,
                    UserId = d.User.UserId,
                    FullName = d.User.Firstname + " " + d.User.Lastname,
                    Email = d.User.Email,
                    Phone = d.User.Phone,
                    City = d.User.City,
                    Image = d.User.Image,
                    UserStatus = d.User.Status.ToString(),
                    RejectResion = d.User.RejectionReason,

                    ExperienceYears = d.ExperienceYears,
                    AvailabilityStatus = d.AvailabilityStatus.ToString(),
                    VehicleType = d.VehicleTypes.ToString(),
                    DriverStatus = d.Status.ToString(),

                    Documents = d.DriverDocuments.Select(doc => new DriverDocumentDto
                    {
                        DocumentId = doc.DocumentId,
                        DocumentType = doc.DocumentType.ToString(),
                        DocumentPath = doc.DocumentPath,
                        Status = doc.Status.ToString()
                    }).ToList()
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                data = drivers
            });
        }


        [HttpGet("/getAprovedDriver")]
        public async Task<IActionResult> GetAllAprovedDriversForAdmin()
        {
            var drivers = await _context.Drivers
                .Include(d => d.User)
                .Include(d => d.DriverDocuments)
                .Where(d => d.User.Status == Model.Entities.User.UserStatus.Active)
                .Select(d => new DriverAdminListDto
                {
                    DriverId = d.DriverId,
                    UserId = d.User.UserId,
                    FullName = d.User.Firstname + " " + d.User.Lastname,
                    Email = d.User.Email,
                    Phone = d.User.Phone,
                    City = d.User.City,
                    Image = d.User.Image,
                    UserStatus = d.User.Status.ToString(),
                    RejectResion = d.User.RejectionReason,

                    ExperienceYears = d.ExperienceYears,
                    AvailabilityStatus = d.AvailabilityStatus.ToString(),
                    VehicleType = d.VehicleTypes.ToString(),
                    DriverStatus = d.Status.ToString(),

                    Documents = d.DriverDocuments.Select(doc => new DriverDocumentDto
                    {
                        DocumentId = doc.DocumentId,
                        DocumentType = doc.DocumentType.ToString(),
                        DocumentPath = doc.DocumentPath,
                        Status = doc.Status.ToString()
                    }).ToList()
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                data = drivers
            });
        }


        [HttpPost("approve-driver/{userId}")]
        public async Task<IActionResult> ApproveDriver(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            user.Status = Model.Entities.User.UserStatus.Active;
            user.RejectionReason = null;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Driver approved successfully"
            });

        }

        [HttpPost("reject-driver")]
        public async Task<IActionResult> RejectDriver([FromBody] RejectDriverDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RejectionReason))
                return BadRequest(new { message = "Rejection reason is required" });

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == dto.UserId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            user.Status = Model.Entities.User.UserStatus.Disabled;
            user.RejectionReason = dto.RejectionReason;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Driver rejected successfully"
            });
        }



        



        [HttpGet("DrivertripsDetails")]
        public async Task<IActionResult> GetDriverTripsDetails()
        {
            // Fetching data from DriverBookingHistories table
            var tripsDetails = await _context.DriverBookingHistories
                .Include(dbh => dbh.Booking)  
                .ThenInclude(b => b.User)  
                .Include(dbh => dbh.Driver)  
                .ThenInclude(d => d.User)  
                .Include(dbh => dbh.Vehicle)  
                .Select(dbh => new
                {
                    DriverFullName = dbh.Driver.User.Firstname + " " + dbh.Driver.User.Lastname,
                    DriverPhone = dbh.Driver.User.Phone,
                    DriverEmail = dbh.Driver.User.Email,
                    DriverId = dbh.DriverId,
                    CustomerFullName = dbh.Booking.User.Firstname + " " + dbh.Booking.User.Lastname,
                    CustomerPhone = dbh.Booking.User.Phone,
                    CustomerEmail = dbh.Booking.User.Email,
                    CustomerCity=dbh.Booking.User.City,
                    VehicleType = dbh.Vehicle.Vehicletype.ToString(),
                    CarName = dbh.Vehicle.CarName,
                    BikeName = dbh.Vehicle.BikeName,
                    RegistrationNo = dbh.Vehicle.RegistrationNo,
                    StartTime = dbh.StartTime,
                    EndTime = dbh.EndTime,
                    Status = dbh.Status, 
                    AssignTime = dbh.AssignTime
                })
                .ToListAsync();

            if (tripsDetails == null || !tripsDetails.Any())
            {
                return NotFound(new { success = false, message = "No trips found." });
            }

            return Ok(new { success = true, data = tripsDetails });
        }



        [HttpGet("DrivertripsDetails/{driverId}")]
        public async Task<IActionResult> GetDriverTripsDetails(int driverId)
        {
            // Fetching data from DriverBookingHistories table filtered by DriverId
            var tripsDetails = await _context.DriverBookingHistories
                .Where(dbh => dbh.DriverId == driverId)  // Filter by DriverId
                .Include(dbh => dbh.Booking)  // Include Booking table
                    .ThenInclude(b => b.User)  // Include Customer details from Booking
                .Include(dbh => dbh.Driver)  // Include Driver table
                    .ThenInclude(d => d.User)  // Include Driver user details
                .Include(dbh => dbh.Vehicle)  // Include Vehicle table
                .Include(dbh => dbh.Vehicle.VehicleImages)  // Include Vehicle Images table
                .Include(dbh => dbh.Vehicle.PricingRule)  // Include Vehicle Pricing table
                .Include(dbh => dbh.Booking.SecurityDeposit)  // Include Security Deposit table
                .Select(dbh => new
                {
                    // Driver Details
                    Driver = new
                    {
                        DriverFullName = dbh.Driver.User.Firstname + " " + dbh.Driver.User.Lastname,
                        DriverPhone = dbh.Driver.User.Phone,
                        DriverEmail = dbh.Driver.User.Email,
                        DriverId = dbh.DriverId
                    },

                    // Customer Details
                    Customer = new
                    {
                        CustomerFullName = dbh.Booking.User.Firstname + " " + dbh.Booking.User.Lastname,
                        CustomerPhone = dbh.Booking.User.Phone,
                        CustomerEmail = dbh.Booking.User.Email,
                        CustomerCity = dbh.Booking.User.City
                    },

                    // Vehicle Details
                    Vehicle = new
                    {
                        VehicleType = dbh.Vehicle.Vehicletype.ToString(),
                        CarName = dbh.Vehicle.CarName,
                        BikeName = dbh.Vehicle.BikeName,
                        RegistrationNo = dbh.Vehicle.RegistrationNo,
                        // Vehicle Image (First image in array or null)
                        VehicleImages = dbh.Vehicle.VehicleImages.Select(vi => vi.ImagePath).ToList(),
                        // Pricing Details
                        Pricing = new
                        {
                            PricePerDay = dbh.Vehicle.PricingRule != null ? dbh.Vehicle.PricingRule.PricePerDay : 0,
                            PricePerHour = dbh.Vehicle.PricingRule != null ? dbh.Vehicle.PricingRule.PricePerHour : 0,
                            PricePerKm = dbh.Vehicle.PricingRule != null ? dbh.Vehicle.PricingRule.PricePerKm : 0
                        }
                    },

                    // Security Deposit
                    SecurityDepositAmount = dbh.Booking.SecurityDeposit != null ? dbh.Booking.SecurityDeposit.Amount : 0,

                    // Booking and Trip Details
                    Booking = new
                    {
                        StartTime = dbh.Booking.StartTime,
                        EndTime = dbh.Booking.EndTime,
                        Status = dbh.Status.ToString(),
                        AssignTime = dbh.AssignTime
                    }
                })
                .ToListAsync();

            if (tripsDetails == null || !tripsDetails.Any())
            {
                return NotFound(new { success = false, message = "No trips found for the given driver." });
            }

            return Ok(new { success = true, data = tripsDetails });
        }


    }
}
