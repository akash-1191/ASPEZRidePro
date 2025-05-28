using EZRide_Project.DTO;
using EZRide_Project.Helpers;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EZRide_Project.Controllers
{
    [Route("api/")]
    [ApiController]
    public class SecurityDepositController : ControllerBase
    {
        private readonly ISecurityDepositService _service;

        public SecurityDepositController(ISecurityDepositService service)
        {
            _service = service;
        }

        [HttpPost("addSecurityDepositPayment")]
        [Authorize]
        public async Task<IActionResult> AddDeposit([FromBody] SecurityDepositDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponseHelper.ValidationFailed("Invalid request data."));

            var response = await _service.AddDepositAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        //get data using the userid security deposit 
        [HttpGet("SecurityDeposit/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserSecurityDeposits(int userId)
        {
            var deposits = await _service.GetUserSecurityDepositsAsync(userId);

            if (deposits == null || deposits.Count == 0)
                return NotFound(ApiResponseHelper.NotFound("Security Deposits"));

            return Ok(ApiResponseHelper.Success(data: deposits));
        }
    }   
}
