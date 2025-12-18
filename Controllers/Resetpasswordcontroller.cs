using EZRide_Project.Data;
using EZRide_Project.DTO.Driver_DTO;
using EZRide_Project.Helpers;
using EZRide_Project.Repositories;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Resetpasswordcontroller : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public Resetpasswordcontroller(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // ================= FORGOT PASSWORD =================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (user == null)
                return Ok("If email exists, reset link will be sent."); // Security best practice

            string token = Guid.NewGuid().ToString();

            user.ResetPasswordToken = token;
            user.ResetPasswordTokenExpiry = DateTime.Now.AddMinutes(30);

            await _context.SaveChangesAsync();

            string resetLink = $"https://localhost:4200/reset-password?token={token}";

            string body = $@"
                <h3>EZRide Password Reset</h3>
                <p>Click below link to reset your password:</p>
                <a href='{resetLink}'>Reset Password</a>
                <p>This link will expire in 30 minutes.</p>
            ";

            await _emailService.SendEmailAsync(
                user.Email,
                "EZRide - Reset Password",
                body
            );

            return Ok("Password reset link sent to your email.");
        }

        // ================= RESET PASSWORD =================
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x =>
                x.ResetPasswordToken == dto.Token &&
                x.ResetPasswordTokenExpiry > DateTime.Now
            );

            if (user == null)
                return BadRequest("Invalid or expired token.");

            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiry = null;

            await _context.SaveChangesAsync();

            return Ok("Password reset successful.");
        }
    }

}
