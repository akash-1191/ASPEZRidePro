using EZRide_Project.DTO;
using EZRide_Project.Helpers;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost("addFeedbackmsg")]
        [Authorize]
        public async Task<IActionResult> AddFeedback([FromBody] FeedbackDTO feedbackDto)
        {
            try
            {
                if (feedbackDto == null)
                    return BadRequest(ApiResponseHelper.Fail("Feedback data cannot be null."));

                var feedback = await _feedbackService.AddFeedbackAsync(feedbackDto);

                return Ok(ApiResponseHelper.Success("Feedback added successfully.", feedback));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseHelper.Fail($"Invalid feedback type. {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseHelper.ServerError(ex.Message));
            }
        }


        [HttpGet("allFeedback")]
        [Authorize]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            try
            {
                var feedbacks = await _feedbackService.GetAllFeedbacksAsync();
                return Ok(ApiResponseHelper.Success("Feedback list fetched successfully.", feedbacks));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponseHelper.ServerError(ex.Message));
            }
        }
    }
}
