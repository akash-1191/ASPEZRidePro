using EZRide_Project.Data;
using EZRide_Project.DTO.Driver_DTO;
using EZRide_Project.Model.Entities;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DriverPaymentsController : ControllerBase
    {
        private readonly IDriverPaymentService _service;
        private readonly IConfiguration _config;

        public DriverPaymentsController(
            IDriverPaymentService service,
            IConfiguration config)
        {
            _service = service;
            _config = config;
        }

        // 🔹 Create Razorpay Order
        [HttpPost("CreateOrder")]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateOrder([FromBody] DriverCreateOrderDto dto)
        {
            RazorpayClient client = new RazorpayClient(
                _config["Razorpay:Key"],
                _config["Razorpay:Secret"]
            );

            var options = new Dictionary<string, object>
        {
            { "amount", dto.Amount * 100 },
            { "currency", "INR" },
            { "receipt", Guid.NewGuid().ToString() }
        };

            Order order = client.Order.Create(options);

            return Ok(new
            {
                orderId = order["id"].ToString(),
                amount = dto.Amount,
                currency = "INR"
            });
        }

        // 🔹 Verify & Save Payment
        [HttpPost("VerifyPayment")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerifyPayment(
            [FromBody] DriverPaymentVerifyDto dto)
        {
            var result = await _service.VerifyPaymentAsync(dto);

            if (!result)
                return BadRequest("Payment verification failed");

            return Ok(new { message = "Driver Payment Saved Successfully" });
        }

    }
}
