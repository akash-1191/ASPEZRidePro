using System.Security.Cryptography;
using System.Text;
using EZRide_Project.Data;
using EZRide_Project.DTO.Driver_DTO;
using EZRide_Project.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Services
{
    public class DriverPaymentService : IDriverPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public DriverPaymentService(
            ApplicationDbContext context,
            IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<bool> VerifyPaymentAsync(DriverPaymentVerifyDto dto)
        {
            // Step 1: Verify Signature
            if (!VerifySignature(dto))
                return false;

            // Step 2: Ensure BookingId exists in the Bookings table
            var booking = await _context.Bookings.FindAsync(dto.BookingId);
            if (booking == null)
            {
                throw new Exception($"Booking with ID {dto.BookingId} not found.");
            }

            // Step 3: Create and save the payment
            var payment = new DriverPayment
            {
                DriverId = dto.DriverId,
                BookingId = dto.BookingId,
                Amount = dto.Amount,
                PaymentType = "Online",
                Status = "Paid",
                CreatedAt = DateTime.Now,
                PaidAt = DateTime.Now
            };

            await _context.DriverPayments.AddAsync(payment);
            await _context.SaveChangesAsync();

            return true;
        }


        private bool VerifySignature(DriverPaymentVerifyDto dto)
        {
            string keySecret = _config["Razorpay:Secret"];
            string payload =
                dto.RazorpayOrderId + "|" + dto.RazorpayPaymentId;

            using var hmac =
                new HMACSHA256(Encoding.UTF8.GetBytes(keySecret));

            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var generatedSignature =
                BitConverter.ToString(hash).Replace("-", "").ToLower();

            return generatedSignature == dto.RazorpaySignature;
        }

    }
}
