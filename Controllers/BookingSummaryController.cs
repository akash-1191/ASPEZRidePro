using System.Security.Claims;
using EZRide_Project.DTO;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingSummaryController : ControllerBase
    {
        private readonly IBookingSummaryService _bookingSummaryService;

        public BookingSummaryController(IBookingSummaryService bookingSummaryService)
        {
            _bookingSummaryService = bookingSummaryService;
        }

        [HttpGet("total-bookings/{userId}")]
        [Authorize]
        public ActionResult<BookingSummaryDTO> GetTotalBookingsByUser(int userId)
        {
            var result = _bookingSummaryService.GetTotalBookingsByUserId(userId);
            return Ok(result);
        }



        [HttpGet("booked-vehicle-type-count/{userId}")]
        [Authorize]
        public async Task<ActionResult<VehicleBookingCountDTO>> GetBookedVehicleTypeCount(int userId)
        {
            var result = await _bookingSummaryService.GetBookedVehicleTypeCountAsync(userId);
            return Ok(result);
        }


        [HttpGet("available-vehicle-count")]
        [Authorize]
        public IActionResult GetAvailableVehicleCount()
        {

            var count = _bookingSummaryService.GetAvailableVehicleCount();
            return Ok(count);
        }

        [HttpGet("pending-payment-count/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetPendingPaymentCount(int userId)
        {
            var count = await _bookingSummaryService.GetPendingPaymentCountAsync(userId);
            return Ok(count);
        }


        [HttpGet("latest-refund/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetLatestRefund(int userId)
        {
            var result = await _bookingSummaryService.GetLatestRefundAsync(userId);
            if (result == null)
                return NotFound("No refund found for this user.");

            return Ok(result);
        }

    }
}
