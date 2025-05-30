using EZRide_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EZRide_Project.Controllers
{
    [Route("api/")]
    [ApiController]
    public class BookingPaymentDetails : ControllerBase
    {
        private readonly IPaymentDetailsService _paymentDetailsService;

        public BookingPaymentDetails(IPaymentDetailsService paymentDetailsService)
        {
            _paymentDetailsService = paymentDetailsService;
        }

        [HttpGet("user-PaymentDetails/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserBookingPayments(int userId)
        {
            var response = await _paymentDetailsService.GetUserBookingPaymentsAsync(userId);
            if (response.IsSuccess)
                return Ok(response);
            return NotFound(response);
        }
        
    }
}
