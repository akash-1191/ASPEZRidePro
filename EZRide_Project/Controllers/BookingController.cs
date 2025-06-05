using System.Security.Claims;
using EZRide_Project.DTO;
using EZRide_Project.Helpers;
using EZRide_Project.Model.Entities;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("addbooking")]
        [Authorize]
        public IActionResult AddBooking([FromBody] BookingDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(ApiResponseHelper.UserDataNull());
            }

            var response = _bookingService.AddBooking(dto);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPut("cancelbooking/{bookingId}")]
        [Authorize]
        public IActionResult CancelBooking(int bookingId, [FromQuery] int userId)
        {
            var response = _bookingService.CancelBooking(bookingId, userId);
            return StatusCode(response.StatusCode, response);
        }


        //get api for all data to get 
        [HttpGet("my-bookings")]
        [Authorize]
        public async Task<IActionResult> GetMyBookings()
        {
            var userIdString = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized("User id missing in token");

            if (!int.TryParse(userIdString, out var userId))
                return BadRequest("Invalid user id in token");

            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            return Ok(bookings);
        }



        [HttpPost("my-bookings/filter")]
        
        public async Task<IActionResult> FilterBookings([FromBody] BookingFilterDTO filter)
        {
            var userIdString = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized("User id missing in token");
            if (!int.TryParse(userIdString, out var userId)) return BadRequest("Invalid user id in token");

            var bookings = await _bookingService.FilterUserBookingsAsync(userId, filter);
            return Ok(bookings);
        }

        //check the booking is avalible or not live

        [HttpGet("availability")]
        public async Task<IActionResult> GetAvailability(int vehicleId, DateTime startDateTime, DateTime endDateTime)
        {
            var availability = await _bookingService.GetAvailabilityAsync(vehicleId, startDateTime, endDateTime);
            return Ok(availability);
        }
    }
}
