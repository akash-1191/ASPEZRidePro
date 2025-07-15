using EZRide_Project.Data;
using EZRide_Project.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminShowFeedBackApi : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminShowFeedBackApi(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET api/feedback
        [HttpGet]
        public IActionResult GetAllFeedback()
        {
            var feedbacks = _context.Feedbacks
                .Include(f => f.User)
                .Select(f => new FeedbackShowDTO
                {
                    FeedbackId = f.FeedbackId,
                    UserId = f.UserId,
                    FeedbackType = f.FeedbackType.ToString(),
                    Message = f.Message,
                    Status = f.Status.ToString(),
                    CreatedAt = f.CreatedAt,
                    User = f.User == null ? null : new UserDTO
                    {
                        UserId = f.User.UserId,
                        Firstname = f.User.Firstname,
                        Middlename = f.User.Middlename,
                        Lastname = f.User.Lastname,
                        Age = f.User.Age,
                        Gender = f.User.Gender,
                        Image = f.User.Image,
                        City = f.User.City,
                        State = f.User.State,
                        Email = f.User.Email,
                        Phone = f.User.Phone,
                        Address = f.User.Address,
                        Status = f.User.Status.ToString(),
                        CreatedAt = f.User.CreatedAt
                    }
                })
                .ToList();

            return Ok(feedbacks);
        }
    }
}
