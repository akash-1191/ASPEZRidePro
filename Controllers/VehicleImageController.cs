using EZRide_Project.DTO;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EZRide_Project.Controllers
{
    [Route("api/")]
    [ApiController]
    public class VehicleImageController : ControllerBase
    {
        private readonly IVehicleImageService _vehicleImageService;

        public VehicleImageController(IVehicleImageService vehicleImageService)
        {
            _vehicleImageService = vehicleImageService;
        }



        //Add image
        [HttpPost("uploadVehicleImage")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadVehicleImage([FromForm] VehicleImageDTO dto)
        {
            var result = await _vehicleImageService.UploadVehicleImageAsync(dto);
            return StatusCode(result.StatusCode, result);
        }


        //updatae image 

        [HttpPut("updateVehicleImage")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateVehicleImage([FromForm] VehicleImageUpdateDTO dto)
        {
            var result = await _vehicleImageService.UpdateVehicleImageAsync(dto);
            return StatusCode(result.StatusCode, result);
        }


        //delete data image
        [HttpDelete("deleteVehicleImage/{imageId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVehicleImage(int imageId)
        {
            var result = await _vehicleImageService.DeleteVehicleImageAsync(imageId);
            return StatusCode(result.StatusCode, result);
        }
        //get aal image 

        [HttpGet("getImagesByVehicle/{vehicleId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetImagesByVehicleId(int vehicleId)
        {
            var images = await _vehicleImageService.GetImagesByVehicleIdAsync(vehicleId);

            if (images == null || !images.Any())
                return NotFound("No images found for this vehicle.");

            return Ok(images);
        }
    }
}
