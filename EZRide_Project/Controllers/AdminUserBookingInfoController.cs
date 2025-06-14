using EZRide_Project.Data;
using EZRide_Project.DTO;
using EZRide_Project.Helpers;
using EZRide_Project.Model.Entities;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Controllers
{
    [Route("api/")]
    [ApiController]

    public class AdminUserBookingInfoController : ControllerBase
    {
        private readonly IAdminUserBookingInfoService _service;
        private readonly ApplicationDbContext _context;
        public AdminUserBookingInfoController(IAdminUserBookingInfoService service, ApplicationDbContext applicationDbContext)
        {
            _service = service;
            _context = applicationDbContext;
        }

        [HttpGet("user-booking-info")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserBookingInfo()
        {
            var data = await _service.GetUserBookingInfoAsync();
            return Ok(data);
        }


        [HttpPut("cancel-reason")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCancelReason([FromBody] BookingCancelReasonDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Cancelreasion))
                return BadRequest("Cancel reason cannot be empty.");

            var booking = await _context.Bookings.FindAsync(dto.BookingId);

            if (booking == null)
                return NotFound("Booking not found.");

            booking.Cancelreasion = dto.Cancelreasion;

            if (booking.Status != Booking.BookingStatus.Cancelled)
            {
                booking.Status = Booking.BookingStatus.Cancelled;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cancel reason updated successfully." });
        }


        


        [HttpPut("status/inprogress")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetBookingStatusToInProgress([FromBody] handOverTheVehicle dto)
        {
            var booking = await _context.Bookings.FindAsync(dto.BookingId);

            if (booking == null)
                return NotFound(ApiResponseHelper.NotFound("Booking"));

            if (booking.Status == Booking.BookingStatus.Cancelled)
                return BadRequest(ApiResponseHelper.Fail("Journey is already cancelled. Cannot set to InProgress."));

            if (booking.Status == Booking.BookingStatus.InProgress)
                return Ok(ApiResponseHelper.Success("Booking is already in InProgress state."));

            booking.Status = Booking.BookingStatus.InProgress;
            await _context.SaveChangesAsync();

            return Ok(ApiResponseHelper.Success("Booking status is InProgress."));
        }



        //user side to print data 
        // CancelledBookings by the admin to the user side show this data
        [HttpGet("CancelledBookings")]
        public IActionResult GetCancelledBookingsByUser()
        {

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            int userId = int.Parse(userIdClaim.Value);

            var cancelledBookings = _context.Bookings
                .Where(b => b.Cancelreasion != null && b.UserId == userId)
                .Include(b => b.Vehicle)
                .ToList()
                .Select(b => new
                {
                    b.BookingId,
                    b.BookingType,
                    b.Cancelreasion,
                    b.CreatedAt,
                    b.StartTime,
                    b.EndTime,
                    b.TotalAmount,
                    b.Status,
                    CarName = b.Vehicle?.CarName,
                    BikeName = b.Vehicle?.BikeName,
                    VehicleType = b.Vehicle?.Vehicletype.ToString(),
                    RegistrationNo = b.Vehicle?.RegistrationNo
                })
                .ToList();

            return Ok(cancelledBookings);
        }


        //user side print data
        //user seen the your runing data

        [HttpGet("bookings/inprogressStatus")]
        [Authorize]
        public IActionResult GetInProgressBookingsForUser()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return Unauthorized(ApiResponseHelper.Unauthorized("User ID not found in token."));
            }

            int userId = int.Parse(userIdClaim.Value);

            var inProgressBookings = _context.Bookings
                .Where(b => b.Status == Booking.BookingStatus.InProgress && b.UserId == userId)
                .Include(b => b.Vehicle)
                .ToList()
                .Select(b => new
                {
                    b.BookingId,
                    b.BookingType,
                    b.CreatedAt,
                    b.StartTime,
                    b.EndTime,
                    b.TotalAmount,
                    Status = b.Status.ToString(),
                    CarName = b.Vehicle?.CarName,
                    BikeName = b.Vehicle?.BikeName,
                    VehicleType = b.Vehicle?.Vehicletype.ToString(),
                    RegistrationNo = b.Vehicle?.RegistrationNo
                })
                .ToList();

            if (!inProgressBookings.Any())
            {
                return NotFound(ApiResponseHelper.NotFound("InProgress bookings"));
            }

            return Ok(ApiResponseHelper.Success("InProgress bookings retrieved successfully", inProgressBookings));
        }



    }
}
