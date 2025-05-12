using System.Security.Claims;
using EZRide_Project.Data;
using EZRide_Project.DTO;
using EZRide_Project.Helpers;
using EZRide_Project.Model;
using EZRide_Project.Model.Entities;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace EZRide_Project.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        public ApiController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }




        //SignUp API
        [HttpPost("Signup")]
        public  IActionResult SignUp(AddUserDataDTO addUserDataDTO)
        {
            if (addUserDataDTO == null || addUserDataDTO.Image == null)
            {
                return BadRequest(ApiResponseHelper.UserDataNull());
            }

            // Email  check 
            var existingUser = dbContext.Users.FirstOrDefault(u => u.Email == addUserDataDTO.Email);
            if (existingUser != null)
            {
                return BadRequest(ApiResponseHelper.EmailAlreadyExists());
            }

            // Upload folder path
            string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Upload_image");
          
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(addUserDataDTO.Image.FileName);
            string filePath = Path.Combine(uploadFolder, uniqueFileName);
            //Console.WriteLine("Full Path: " + filePath);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                addUserDataDTO.Image.CopyTo(stream);
            }



            String HasPassword = BCrypt.Net.BCrypt.HashPassword(addUserDataDTO.Password);

            var NewUser = new User()
            {
                RoleId = addUserDataDTO.RoleId,
                Firstname = addUserDataDTO.Firstname,
                Lastname = addUserDataDTO.Lastname,
                Middlename = addUserDataDTO.Middlename,
                Email = addUserDataDTO.Email,
                Address = addUserDataDTO.Address,
                Password = HasPassword,
                Phone = addUserDataDTO.Phone,
                Age = addUserDataDTO.Age,
                Gender = addUserDataDTO.Gender,
                Image = "/Upload_image" + uniqueFileName,
                City = addUserDataDTO.City,
                State = addUserDataDTO.State,
                CreatedAt = DateTime.Now
            };
            dbContext.Users.Add(NewUser);
            dbContext.SaveChanges();
            return Ok(ApiResponseHelper.Success("User registered successfully."));
        }




        //Login Api 
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO, [FromServices] JwtTokenGenerator jwtTokenGenerator, [FromServices] EmailService _emailService)
        {
            if (loginDTO == null || string.IsNullOrWhiteSpace(loginDTO.Email) || string.IsNullOrWhiteSpace(loginDTO.Password))
            {
                return BadRequest(new { Message = "Invalid input. Email and Password are required." });
            }

            var user = dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == loginDTO.Email);

            if (user == null)
            {

                return Unauthorized(new { Message = "Invalid credentials." });
            }

            try
            {
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password);

                if (!isPasswordValid)
                {
                    return Unauthorized(new { Message = "Invalid credentials." });
                }

                string token = jwtTokenGenerator.GenerateToken(user.Email, user.RoleId, user.UserId);
                await _emailService.SendEmailAsync(
               user.Email,
               "Login Successful",
               $"Hello {user.Firstname}\n{user.Middlename}\n{user.Lastname},\n\nYou have logged in successfully at {DateTime.Now}."
           );

                return Ok(new
                {
                    Token = token,
                    Message = "Login successful",
                    User = new
                    {
                        user.UserId,
                        user.Email,
                        user.RoleId,
                        RoleName = user.Role.RoleName.ToString()
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during login.", Details = ex.Message });
            }
        }




        [HttpGet("Profile/{id}")]
        [Authorize]
        public IActionResult GetUserProfile(int id)
        {
            // Correct way to retrieve custom claim
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (userIdClaim == null)
                {
                return Unauthorized(new ApiResponseModel
                {
                    IsSuccess = false,
                    Message = "Unauthorized access.",
                    StatusCode = 401
                });
            }

            int userId = int.Parse(userIdClaim);

            if (userId != id)
            {
                return Forbid();
            }

            var user = dbContext.Users.FirstOrDefault(u => u.UserId == id);

            if (user == null)
            {
                return NotFound(new ApiResponseModel
                {
                    IsSuccess = false,
                    Message = "User not found.",
                    StatusCode = 404
                });
            }

            var userProfile = new UserProfileDTO
            {
                UserId = user.UserId,
                Firstname = user.Firstname,
                Middlename = user.Middlename,
                Lastname = user.Lastname,
                Email = user.Email,
                Address = user.Address,
                Phone = user.Phone,
                Age = user.Age,
                Gender = user.Gender,
                Image = user.Image,
                City = user.City,
                State = user.State,
                RoleId = user.RoleId,
                CreatedAt = user.CreatedAt
            };

            return Ok(new
            {
                IsSuccess = true,
                Message = "User profile fetched successfully.",
                StatusCode = 200,
                Data = userProfile
            });
        }

    }
}
