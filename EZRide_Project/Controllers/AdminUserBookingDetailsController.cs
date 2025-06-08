using EZRide_Project.DTO;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminUserBookingDetailsController : ControllerBase
    {
        private readonly IUserBookingDetailsService _service;

        public AdminUserBookingDetailsController(IUserBookingDetailsService service)
        {
            _service = service;
        }

        // GET: api/UserBookingDetails/all
        [HttpGet("all")]
        public async Task<ActionResult<List<UserBookingDetailsDto>>> GetAllUsersBookingDetails()
        {
            var result = await _service.GetAllUsersBookingDetailsAsync();
            if (result == null || result.Count == 0)
                return NotFound("No users found.");

            return Ok(result);
        }
    }
}
