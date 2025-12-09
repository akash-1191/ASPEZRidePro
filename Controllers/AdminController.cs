using EZRide_Project.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] 
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext db, ILogger<AdminController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // Get users by role (for admin chat)
        [HttpGet("users/{role}")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            try
            {
                // Convert role string to match your enum
                string roleEnumValue = role switch
                {
                    "owner" => "OwnerVehicle",
                    "customer" => "Customer",
                    "driver" => "Driver",
                    _ => role 
                };

                var users = await _db.Users
                    .Include(u => u.Role)
                    .Where(u => u.Role.RoleName.ToString() == roleEnumValue &&
                                u.Status == EZRide_Project.Model.Entities.User.UserStatus.Active)
                    .Select(u => new
                    {
                        u.UserId,
                        u.Firstname,
                        u.Lastname,
                        u.Email,
                        u.Phone,
                        FullName = $"{u.Firstname} {u.Lastname}",
                        Role = u.Role.RoleName.ToString(),
                        Status = u.Status.ToString()
                    })
                    .OrderBy(u => u.Firstname)
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by role: {Role}", role);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // Get all users (optional)
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _db.Users
                    .Include(u => u.Role)
                    .Where(u => u.Status == EZRide_Project.Model.Entities.User.UserStatus.Active)
                    .Select(u => new
                    {
                        u.UserId,
                        u.Firstname,
                        u.Lastname,
                        u.Email,
                        u.Phone,
                        FullName = $"{u.Firstname} {u.Lastname}",
                        Role = u.Role.RoleName.ToString(),
                        Status = u.Status.ToString()
                    })
                    .OrderBy(u => u.Role)
                    .ThenBy(u => u.Firstname)
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // Get user statistics (optional)
        [HttpGet("user-stats")]
        public async Task<IActionResult> GetUserStats()
        {
            try
            {
                var stats = await _db.Users
                    .Include(u => u.Role)
                    .Where(u => u.Status == EZRide_Project.Model.Entities.User.UserStatus.Active)
                    .GroupBy(u => u.Role.RoleName.ToString())
                    .Select(g => new
                    {
                        Role = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user stats");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
