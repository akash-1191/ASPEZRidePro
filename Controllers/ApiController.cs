using System.Drawing;
using System.Security.Claims;
using EZRide_Project.Data;
using EZRide_Project.DTO;
using EZRide_Project.Helpers;
using EZRide_Project.Model;
using EZRide_Project.Model.Entities;
using EZRide_Project.Services;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace EZRide_Project.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly EmailService _emailService;
        public ApiController(IUserService userService, EmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        //Signupdata
        [HttpPost("signup")]
        public IActionResult SignUp([FromForm] AddUserDataDTO dto)
        {
            var response = _userService.RegisterUser(dto);
            return StatusCode(response.StatusCode, response);
        }

        //Login Api
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var response =await _userService.LoginUser(loginDTO,_emailService);
            return StatusCode(response.StatusCode, response);
        }

        //GetuserData Api
        [HttpGet("profile/{id}")]
        [Authorize]
        public IActionResult GetUserProfile(int id)
        {

            var claim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (claim == null)
                return Unauthorized(ApiResponseHelper.Unauthorized());

            int authUserId = int.Parse(claim);

            var response = _userService.GetUserProfile(authUserId, id);

            return StatusCode(response.StatusCode, response);
        }

        //UpdateUserData Api
        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateUserData([FromBody] UserProfileUpdateDTO model)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var claim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (claim == null)
                return Unauthorized(ApiResponseHelper.Unauthorized());


            int authUserId = int.Parse(claim);
        
            var result =_userService.UpdateUserProfile(authUserId, model);
            return Ok(result);
        }


        //User profile Update

        [HttpPut("update-profile-image")]
        [Authorize]
        public async Task<IActionResult> UpdateProfileImage([FromForm] UpdateUserImageDTO updateUserImageDTO)
        {
            var result=await _userService.UpdateUserImageAsync(updateUserImageDTO);
            return StatusCode(result.StatusCode, result);
        }

    }
}
