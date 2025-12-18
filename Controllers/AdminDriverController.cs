using EZRide_Project.Data;
using EZRide_Project.DTO.Driver_DTO;
using EZRide_Project.Model.Entities;
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
                    CustomerCity = dbh.Booking.User.City,
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

            var tripsDetails = await _context.DriverBookingHistories
                .Where(dbh => dbh.DriverId == driverId)
                .Include(dbh => dbh.Booking)
                .ThenInclude(b => b.User)
                .Include(dbh => dbh.Driver)
                .ThenInclude(d => d.User)
                .Include(dbh => dbh.Vehicle)
                .Include(dbh => dbh.Vehicle.VehicleImages)
                .Include(dbh => dbh.Vehicle.PricingRule)
                .Include(dbh => dbh.Booking.SecurityDeposit)
                .Select(dbh => new
                {
                    // Driver Details
                    Driver = new
                    {
                        DriverFullName = dbh.Driver.User.Firstname + " " + dbh.Driver.User.Lastname,
                        DriverPhone = dbh.Driver.User.Phone,
                        DriverEmail = dbh.Driver.User.Email,
                        DriverId = dbh.DriverId,
                        PerDayRate = dbh.Driver.PerDayRate
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
                        AssignTime = dbh.AssignTime,
                        BookingId = dbh.BookingId
                    },
                    DriverPayment = _context.DriverPayments
                        .Where(dp => dp.BookingId == dbh.BookingId && dp.DriverId == dbh.DriverId)
                        .Select(dp => new
                        {
                            dp.Amount,
                            dp.Status,
                            dp.PaidAt
                        })
                        .FirstOrDefault(),

                })
                .ToListAsync();

            if (tripsDetails == null || !tripsDetails.Any())
            {
                return NotFound(new { success = false, message = "No trips found for the given driver." });
            }

            return Ok(new { success = true, data = tripsDetails });
        }



        [HttpPut("UpdatePerDayRate")]
        public async Task<IActionResult> UpdatePerDayRate([FromBody] UpdateDriverRateDTO dto)
        {
            if (dto.PerDayRate <= 0)
                return BadRequest("Per day rate must be greater than 0");

            var driver = await _context.Drivers
                .FirstOrDefaultAsync(d => d.DriverId == dto.DriverId);

            if (driver == null)
                return NotFound("Driver not found");

            driver.PerDayRate = dto.PerDayRate;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Driver per day rate updated successfully",
                DriverId = driver.DriverId,
                PerDayRate = driver.PerDayRate
            });
        }



        [HttpGet("AllPaidDriversPaymentReport")]
        public async Task<IActionResult> GetAllPaidDriversPaymentReport()
        {
            var result = await _context.DriverBookingHistories
                .Include(dbh => dbh.Driver)
                    .ThenInclude(d => d.User)
                .Include(dbh => dbh.Booking)
                .Where(dbh =>
                    _context.DriverPayments.Any(dp =>
                        dp.BookingId == dbh.BookingId &&
                        dp.DriverId == dbh.DriverId &&
                        dp.Status == "Paid"
                    )
                )
                .Select(dbh => new
                {
                    // 🔹 Driver Info
                    Driver = new
                    {
                        DriverId = dbh.DriverId,
                        DriverName = dbh.Driver.User.Firstname + " " + dbh.Driver.User.Lastname,
                        DriverPhone = dbh.Driver.User.Phone,
                        DriverEmail = dbh.Driver.User.Email
                    },

                    // 🔹 Booking Info
                    Booking = new
                    {
                        BookingId = dbh.BookingId,
                        StartTime = dbh.Booking.StartTime,
                        EndTime = dbh.Booking.EndTime,
                        Status = dbh.Status.ToString()
                    },

                    // 🔹 Payment Info (PAID ONLY)
                    DriverPayment = _context.DriverPayments
                        .Where(dp =>
                            dp.DriverId == dbh.DriverId &&
                            dp.BookingId == dbh.BookingId &&
                            dp.Status == "Paid"
                        )
                        .Select(dp => new
                        {
                            dp.DriverPaymentId,
                            dp.Amount,
                            dp.PaymentType,
                            dp.Status,
                            dp.CreatedAt,
                            dp.PaidAt
                        })
                        .FirstOrDefault()
                })
                .OrderByDescending(x => x.DriverPayment.PaidAt)
                .ToListAsync();

            if (!result.Any())
            {
                return NotFound(new
                {
                    success = false,
                    message = "No paid driver payments found"
                });
            }

            return Ok(new
            {
                success = true,
                data = result
            });
        }
    }
}
