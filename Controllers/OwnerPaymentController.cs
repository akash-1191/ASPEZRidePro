using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OwnerPaymentController : ControllerBase
    {
        private readonly IOwnerPaymentService _service;
        private readonly IConfiguration _config;

        public OwnerPaymentController(IOwnerPaymentService service, IConfiguration config)
        {
            _service = service;
            _config = config;
        }

        //  Create Razorpay order
        [HttpPost("CreateOrder")]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateOrder([FromBody] CreateOrderDto dto)
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


        //  Verify & Save & update
        [HttpPost("VerifyPayment")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerifyPayment([FromBody] OwnerPaymentVerifyDto dto)
        {
            var result = await _service.VerifyPaymentAsync(dto);

            if (!result)
                return BadRequest("Signature verification failed");

            return Ok(new { message = "Owner Payment Saved Successfully" });
        }

        //  Owner all payments
        [HttpGet("GetOwnerPayments/{ownerId}")]
        [Authorize(Roles = "Admin,OwnerVehicle")]
        public async Task<IActionResult> GetOwnerPayments(int ownerId)
        {
            var data = await _service.GetOwnerPaymentsAsync(ownerId);
            return Ok(data);
        }
    }
}

