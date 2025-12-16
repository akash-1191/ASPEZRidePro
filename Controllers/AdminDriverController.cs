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



        //[HttpGet("DrivertripsDetails")]
        //public async Task<IActionResult> GetAllOngoingTrips()
        //{
        //    // Fetch ongoing trips for all drivers where the driver status is "Active"
        //    var ongoingTrips = await _context.Bookings
        //        .Where(b => b.Driver.User.Status == Model.Entities.User.UserStatus.Active) // Filter by active driver and ongoing status
        //        .Include(b => b.Driver)
        //        .ThenInclude(d => d.User)
        //        .Include(b => b.Vehicle)
        //        .Include(b => b.User) 
        //        .Select(b => new OngoingTripDTO
        //        {
        //            DriverFullName = b.Driver.User.Firstname + " " + b.Driver.User.Lastname,
        //            DriverPhone = b.Driver.User.Phone,
        //            VehicleType = b.Vehicle.Vehicletype.ToString(),
        //            CarName = b.Vehicle.CarName,
        //            BikeName = b.Vehicle.BikeName,
        //            RegistrationNo=b.Vehicle.RegistrationNo,
        //            DriverId=b.DriverId.Value,
        //            UesrEmail=b.User.Email,
        //            DriverEmail=b.Driver.User.Email,
        //            CustomerFullName = b.User.Firstname + " " + b.User.Lastname,
        //            CustomerPhone = b.User.Phone,
        //            CustomerCity = b.User.City,
        //            StartTime = b.StartTime,
        //            EndTime = b.EndTime,
        //            DriverAvailabiliStatus = b.Driver.AvailabilityStatus.ToString() // Ongoing
        //        })
        //        .ToListAsync();

        //    if (ongoingTrips == null || !ongoingTrips.Any())
        //    {
        //        return NotFound(new { success = false, message = "No ongoing trips found for the drivers." });
        //    }

        //    return Ok(new { success = true, data = ongoingTrips });
        //}



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



    }
}
