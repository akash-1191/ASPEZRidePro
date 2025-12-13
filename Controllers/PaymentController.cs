using EZRide_Project.DTO;
using EZRide_Project.Helpers;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Razorpay.Api;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly string _razorpayKey;
        private readonly string _razorpaySecret;

        public PaymentController(IPaymentService paymentService, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _razorpayKey = configuration["Razorpay:Key"];
            _razorpaySecret = configuration["Razorpay:Secret"];
        }

        [HttpPost("VerifyPayment")]
        public async Task<IActionResult> VerifyPayment([FromBody] RazorpayVerificationDto dto)
        {
            if (dto == null)
                return BadRequest(ApiResponseHelper.Fail("Invalid payment data"));

            bool result = await _paymentService.VerifyAndSavePaymentAsync(dto);

            if (result)
                return Ok(ApiResponseHelper.Success("Payment verified and saved successfully"));
            else
                return BadRequest(ApiResponseHelper.Fail("Payment verification failed"));
        }

        

        [HttpPost("CreateOrder")]
        public IActionResult CreateOrder([FromBody] CreateOrderRequestDto request)
        {
            try
            {
                RazorpayClient client = new RazorpayClient(_razorpayKey, _razorpaySecret);

                var receiptId = $"order_rcptid_{Guid.NewGuid().ToString().Substring(0, 8)}";

                var options = new Dictionary<string, object>
             {
     { "amount", (int)(request.Amount * 100) },  // ✅ amount in paise
     { "currency", "INR" },
     { "receipt", receiptId },
     { "payment_capture", 1 }                   // ✅ auto capture
 };


                Razorpay.Api.Order order = client.Order.Create(options);

                return Ok(new
                {
                    orderId = order["id"].ToString(),
                    amount = Convert.ToInt32(order["amount"]),
                    currency = order["currency"].ToString()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseHelper.Fail("Order creation failed: " + ex.Message));
            }
        }
    }
}

