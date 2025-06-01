using EZRide_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentReceiptController : ControllerBase
    {
        private readonly IPaymentReceiptService _paymentReceiptService;

        public PaymentReceiptController(IPaymentReceiptService paymentReceiptService)
        {
            _paymentReceiptService = paymentReceiptService;
        }

        [HttpGet("download/{userId}/{bookingId}")]
        [Authorize]
        public async Task<IActionResult> DownloadReceipt(int userId, int bookingId)
        {
            var dto = await _paymentReceiptService.GetPaymentReceiptAsync(userId, bookingId);

            if (dto == null)
                return NotFound("No receipt found for the given UserId and BookingId.");

            var pdfBytes = _paymentReceiptService.GeneratePdf(dto);

            return File(pdfBytes, "application/pdf", $"PaymentReceipt_User{userId}_Booking{bookingId}.pdf");
        }


        [HttpPost("send-email")]
        [Authorize]
        public async Task<IActionResult> SendReceiptEmail([FromQuery] int userId, [FromQuery] int bookingId, [FromBody] string email)
        {
            await _paymentReceiptService.SendReceiptEmailAsync(email, userId, bookingId);
            return Ok("Email sent successfully.");
        }
    }
}
