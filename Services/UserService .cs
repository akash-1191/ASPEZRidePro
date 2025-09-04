using EZRide_Project.DTO;
using EZRide_Project.Helpers;
using EZRide_Project.Model;
using EZRide_Project.Model.Entities;
using EZRide_Project.Repositories;
using Humanizer;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.IO;
using System.Linq;

namespace EZRide_Project.Services
{
    public class UserService :IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IWebHostEnvironment _environment;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public UserService(
            IUserRepository repository,
            IWebHostEnvironment environment,
            JwtTokenGenerator jwtTokenGenerator)
        {
            _repository = repository;
            _environment = environment;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public ApiResponseModel RegisterUser(AddUserDataDTO dto)
        {
            if (dto == null || dto.Image == null)
            {
                return ApiResponseHelper.UserDataNull();
            }

            if (_repository.IsEmailExists(dto.Email))
            {
                return ApiResponseHelper.EmailAlreadyExists();
            }

            string[] blockedExtensions = { ".exe", ".bat", ".cmd", ".sh", ".js" };

            var extension = Path.GetExtension(dto.Image.FileName).ToLower();
            if (blockedExtensions.Contains(extension))
            {
                return ApiResponseHelper.Fail("This file type is not allowed.");
            }

            if (_environment.WebRootPath == null)
            {
                return ApiResponseHelper.Fail("WebRootPath is not set correctly.Contact  admin");
            }

            string uploadFolder = Path.Combine(_environment.WebRootPath, "Upload_image");


            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            string uniqueFileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
            string filePath = Path.Combine(uploadFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                dto.Image.CopyTo(stream);
            }


            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                RoleId = dto.RoleId,
                Firstname = dto.Firstname,
                Middlename = dto.Middlename,
                Lastname = dto.Lastname,
                Email = dto.Email,
                Password = hashedPassword,
                Address = dto.Address,
                Phone = dto.Phone,
                Age = dto.Age,
                Gender = dto.Gender,
                City = dto.City,
                State = dto.State,
                CreatedAt = DateTime.Now,
                Image = "/Upload_image/" + uniqueFileName
            };

            _repository.AddUser(user);

            return ApiResponseHelper.Success("User registered successfully.");
        }


        //Login services
        public async Task<ApiResponseModel> LoginUser(LoginDTO loginDTO, EmailService _emailService)
        {
            if (loginDTO == null || string.IsNullOrWhiteSpace(loginDTO.Email) || string.IsNullOrWhiteSpace(loginDTO.Password))
            {
                return ApiResponseHelper.Fail("Email or password must not be empty.");
            }

            var user = _repository.GetUserByEmail(loginDTO.Email);
            if (user == null)
            {
                return ApiResponseHelper.Fail("Invalid email or password.");
            }

            if (user.Email != loginDTO.Email)
            {
                return ApiResponseHelper.Fail("Invalid email or password.");  
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password);
            if (!isPasswordValid)
            {
                return ApiResponseHelper.Fail("Invalid email or password.");
            }

            var token = _jwtTokenGenerator.GenerateToken(user.Email, user.RoleId, user.UserId, user.Role.RoleName.ToString());


            await _emailService.SendEmailAsync(
               user.Email,
               "Login Successful",
               $"Hello {user.Firstname}\n{user.Middlename}\n{user.Lastname},\n\nYou have logged in successfully at {DateTime.Now}."
           );

            return ApiResponseHelper.Success("Login successful", new
            {
                Token = token,
                User = new
                {
                    user.UserId,
                    user.Email,
                    user.RoleId,
                    RoleName = user.Role.RoleName.ToString()
                }           
            });
        }


        //profile data
            public ApiResponseModel GetUserProfile(int authUserId, int requestedUserId)
        {
            
            if (authUserId != requestedUserId)
                return ApiResponseHelper.Forbidden("Access denied.");

            var user = _repository.GetuserProfile(requestedUserId);
            if (user == null)
                return ApiResponseHelper.NotFound("User");

            var dto = new UserProfileDTO
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

            return ApiResponseHelper.Success("User profile fetched successfully.", dto);
        }


        //update user data
        public ApiResponseModel UpdateUserProfile(int authUserId, UserProfileUpdateDTO model)
        {
            if (authUserId != model.UserId)
                return ApiResponseHelper.Forbidden("Access denied.");

            var user = _repository.GetuserById(model.UserId);
            if (user == null)
                return ApiResponseHelper.NotFound("User not found.");

            user.Firstname = model.Firstname;
            user.Middlename = model.Middlename;
            user.Lastname = model.Lastname;
            user.Age = model.Age;
            user.Address = model.Address;
            user.Phone = model.Phone;
            user.Gender = model.Gender;
            user.City = model.City;
            user.State = model.State;

            _repository.UpdateUser(user);

            return ApiResponseHelper.Success("User profile updated successfully.");
        }

        //update the user profile image

        public async Task<ApiResponseModel> UpdateUserImageAsync(UpdateUserImageDTO updateUserImageDTO)
        {
            if (updateUserImageDTO == null || updateUserImageDTO.Image == null)
                return ApiResponseHelper.Fail("Image is required.");

            string[] blockedExtensions = { ".exe", ".bat", ".cmd", ".sh", ".js" };

            var extension = Path.GetExtension(updateUserImageDTO.Image.FileName).ToLower();
            if (blockedExtensions.Contains(extension))
            {
                return ApiResponseHelper.FileNotAllow("This file type is not allowed.");
            }

            var user = await _repository.GetUserByIdAsync(updateUserImageDTO.UserId);
            if (user == null)
                return ApiResponseHelper.NotFound("User not found.");

            if (_environment.WebRootPath == null)
                return ApiResponseHelper.Fail("WebRootPath not set.");

            string uploadFolder = Path.Combine(_environment.WebRootPath, "Upload_image");

            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            // Delete old image
            if (!string.IsNullOrEmpty(user.Image))
            {
                string oldImagePath = Path.Combine(_environment.WebRootPath, user.Image.TrimStart('/'));
                if (File.Exists(oldImagePath))
                    File.Delete(oldImagePath);
            }

            // Save new image
            string uniqueFileName = Guid.NewGuid() + Path.GetExtension(updateUserImageDTO.Image.FileName);
            string newFilePath = Path.Combine(uploadFolder, uniqueFileName);

            using (var stream = new FileStream(newFilePath, FileMode.Create))
            {
                await updateUserImageDTO.Image.CopyToAsync(stream);
            }

            user.Image = "/Upload_image/" + uniqueFileName;

            await _repository.UpdateUserAsync(user);

            return ApiResponseHelper.Success("Profile image updated successfully.");
        }
    }
    
}
