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
    [Route("api/[controller]")]
    [ApiController]
    public class StatusChangewithOTP : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public StatusChangewithOTP(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("send-otp")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendOtp([FromBody] handOverTheVehicle dto)
        {
            var booking = await _context.Bookings.FindAsync(dto.BookingId);
            if (booking == null)
                return NotFound(ApiResponseHelper.NotFound("Booking not found."));

            string otp = new Random().Next(100000, 999999).ToString();

            //  Check for existing unused, unexpired OTP for this booking
            var existingOtp = await _context.BookingOTPs
                .FirstOrDefaultAsync(x => x.BookingId == dto.BookingId && !x.IsUsed && x.ExpiryTime > DateTime.UtcNow);

            if (existingOtp != null)
            {
                //  Update existing OTP
                existingOtp.OTPCode = otp;
                existingOtp.ExpiryTime = DateTime.UtcNow.AddMinutes(5);
                existingOtp.EmailSentTo = dto.AdminEmail;
            }
            else
            {
                //  Create new OTP record
                var bookingOtp = new BookingOTP
                {
                    BookingId = dto.BookingId,
                    OTPCode = otp,
                    EmailSentTo = dto.AdminEmail,
                    ExpiryTime = DateTime.UtcNow.AddMinutes(5),
                    IsUsed = false
                };
                _context.BookingOTPs.Add(bookingOtp);
            }

            await _context.SaveChangesAsync();

            //  Send Email
            string htmlBody = $@"
      <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>

        <div style='text-align: center; margin-bottom: 20px;'>
          <img src='https://res.cloudinary.com/druzdz5zn/image/upload/v1750137448/eZride_mszztm.jpg' alt='EZRide Logo' width='100' style='margin-bottom: 10px;' />
          <h2 style='color: #4A90E2;'>EZRide Vehicle Rental System</h2>
        </div>

        <p style='font-size: 16px;'>Hello,</p>
        <p style='font-size: 16px;'>Your OTP for Booking ID <strong>{dto.BookingId}</strong> is:</p>

        <div style='text-align: center; margin: 30px 0;'>
          <h1 style='font-size: 36px; color: #333;'>{otp}</h1>
        </div>

        <p style='font-size: 14px; color: #555;'>This OTP is valid for 5 minutes. Please do not share it with anyone.</p>

        <hr style='margin: 30px 0;' />

        <footer style='font-size: 12px; color: #888; text-align: center;'>
          Sent by <strong>EZRide Vehicle Rental System</strong><br/>
          © {DateTime.UtcNow.Year} EZRide. All rights reserved.
        </footer>
      </div>";

            await _emailService.SendEmailAsync(
                dto.AdminEmail,
                "OTP from EZRide - Booking Verification",
                htmlBody
            );

            return Ok(ApiResponseHelper.Success("OTP has been sent to the provided email."));
        }

        [HttpPost("verify-otp")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerifyOtpAndUpdateStatus([FromBody] VerifyOtpDto dto)
        {
            var booking = await _context.Bookings.FindAsync(dto.BookingId);
            if (booking == null)
                return NotFound(ApiResponseHelper.NotFound("Booking not found."));

            var otpRecord = await _context.BookingOTPs
                .Where(x => x.BookingId == dto.BookingId && x.OTPCode == dto.OTP)
                .OrderByDescending(x => x.ExpiryTime)
                .FirstOrDefaultAsync();

            if (otpRecord == null)
                return BadRequest(ApiResponseHelper.Fail("Invalid OTP."));

            if (otpRecord.IsUsed)
                return BadRequest(ApiResponseHelper.Fail("OTP has already been used."));

            if (otpRecord.ExpiryTime < DateTime.UtcNow)
                return BadRequest(ApiResponseHelper.Fail("OTP has expired."));

           
            otpRecord.IsUsed = true;

         
            booking.Status = Booking.BookingStatus.InProgress;

            await _context.SaveChangesAsync();

            return Ok(ApiResponseHelper.Success("OTP verified successfully. Booking is now In Progress."));
        }

    }
}
