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
    public class PricingRuleController : ControllerBase
    {
        private readonly IPricingRuleService _service;

        public PricingRuleController(IPricingRuleService service)
        {
            _service = service;
        }

        [HttpPost("set-Or-price")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> SetOrUpdatePrice([FromBody] PricingRuleDto dto)
        {
            if (dto == null)
                return BadRequest(ApiResponseHelper.ValidationFailed("Invalid input."));

            var result = await _service.SetOrUpdatePricingAsync(dto);
            return StatusCode(result.StatusCode, result);
        }



        [HttpGet("get-by-vehicle/{vehicleId}")]
        
        public async Task<IActionResult> GetByVehicleId(int vehicleId)
        {
            var result = await _service.GetPricingByVehicleIdAsync(vehicleId);
            return StatusCode(result.StatusCode, result);
        }
    }
}



