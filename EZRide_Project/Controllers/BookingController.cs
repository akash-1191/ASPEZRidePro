using EZRide_Project.DTO;
using EZRide_Project.Helpers;
using EZRide_Project.Model.Entities;
using EZRide_Project.Services;
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
        public IActionResult CancelBooking(int bookingId, [FromQuery] int userId)
        {
            var response = _bookingService.CancelBooking(bookingId, userId);
            return StatusCode(response.StatusCode, response);
        }


        //get api for all data to get 
        [HttpGet("getbookings/{userId}")]
        public IActionResult GetBookingsByUserId(int userId)
        {
            var response = _bookingService.GetBookingsByUserId(userId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
