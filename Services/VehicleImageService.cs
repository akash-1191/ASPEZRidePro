using EZRide_Project.DTO;
using EZRide_Project.Helpers;
using EZRide_Project.Model.Entities;
using EZRide_Project.Model;
using EZRide_Project.Repositories;

namespace EZRide_Project.Services
{
    public class VehicleImageService : IVehicleImageService
    {
        private readonly IVehicleImageRepository _imageRepository;
        private readonly IWebHostEnvironment _env;

        public VehicleImageService(IVehicleImageRepository imageRepository, IWebHostEnvironment env)
        {
            _imageRepository = imageRepository;
            _env = env;
        }
        //add Image Logic
        public async Task<ApiResponseModel> UploadVehicleImageAsync(VehicleImageDTO dto)
        {
            try
            {
                if (dto.ImageFile == null || dto.ImageFile.Length == 0)
                    return ApiResponseHelper.Fail("Image file is required.");

                var extension = Path.GetExtension(dto.ImageFile.FileName);
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if (!allowedExtensions.Contains(extension.ToLower()))
                    return ApiResponseHelper.FileNotAllow("Only JPG, JPEG, PNG, and WEBP are allowed.");

                var imageName = Guid.NewGuid().ToString() + extension;
                var savePath = Path.Combine(_env.WebRootPath, "Upload_image", "vehicles");

                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);

                var fullPath = Path.Combine(savePath, imageName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }

                var vehicleImage = new VehicleImage
                {
                    VehicleId = dto.VehicleId,
                    ImagePath = $"Upload_image/vehicles/{imageName}",
                    CreatedAt = DateTime.Now
                };

                await _imageRepository.AddVehicleImageAsync(vehicleImage);
                var saved = await _imageRepository.SaveChangesAsync();

                if (!saved)
                    return ApiResponseHelper.Fail("Image not saved to database.");

                return ApiResponseHelper.Success("Vehicle image uploaded successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.ServerError("Exception: " + ex.Message);
            }
        }

        //update image logic

        public async Task<ApiResponseModel> UpdateVehicleImageAsync(VehicleImageUpdateDTO dto)
        {
            try
            {
                if (dto.NewImageFile == null || dto.NewImageFile.Length == 0)
                    return ApiResponseHelper.Fail("New image file is required.");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(dto.NewImageFile.FileName);

                if (!allowedExtensions.Contains(extension.ToLower()))
                    return ApiResponseHelper.FileNotAllow("Only JPG, JPEG, PNG, and WEBP files are allowed.");

                var existingImage = await _imageRepository.GetVehicleImageByIdAsync(dto.VehicleImageId);
                if (existingImage == null)
                    return ApiResponseHelper.NotFound("Vehicle image");

                // Delete old image from disk
                var oldImagePath = Path.Combine(_env.WebRootPath, existingImage.ImagePath.Replace("/", "\\"));
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }

                // Save new image
                var newImageName = Guid.NewGuid().ToString() + extension;
                var folderPath = Path.Combine(_env.WebRootPath, "Upload_image", "vehicles");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var newFullPath = Path.Combine(folderPath, newImageName);
                using (var stream = new FileStream(newFullPath, FileMode.Create))
                {
                    await dto.NewImageFile.CopyToAsync(stream);
                }

                // Update image path in DB
                existingImage.ImagePath = $"Upload_image/vehicles/{newImageName}";
                existingImage.CreatedAt = DateTime.Now;

                _imageRepository.UpdateVehicleImage(existingImage);
                var saved = await _imageRepository.SaveChangesAsync();

                if (!saved)
                    return ApiResponseHelper.Fail("Failed to update image.");

                return ApiResponseHelper.Success("Image updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.ServerError("Exception: " + ex.Message);
            }
        }


        //delete image
        public async Task<ApiResponseModel> DeleteVehicleImageAsync(int vehicleImageId)
        {
            try
            {
                var image = await _imageRepository.GetVehicleImageByIdAsync(vehicleImageId);
                if (image == null)
                    return ApiResponseHelper.NotFound("Vehicle image");


                var filePath = Path.Combine(_env.WebRootPath, image.ImagePath.Replace("/", "\\"));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }


                _imageRepository.DeleteVehicleImage(image);
                var saved = await _imageRepository.SaveChangesAsync();

                if (!saved)
                    return ApiResponseHelper.Fail("Failed to delete image from database.");

                return ApiResponseHelper.Success("Vehicle image deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.ServerError("Exception: " + ex.Message);
            }
        }

        //get all image 

        public async Task<List<VehicleImageResponseDTO>> GetImagesByVehicleIdAsync(int vehicleId)
        {
            var images = await _imageRepository.GetImagesByVehicleIdAsync(vehicleId);

            var result = images.Select(i => new VehicleImageResponseDTO
            {
                VehicleImageId = i.VehicleImageId,
                VehicleId = i.VehicleId,
                ImagePath = i.ImagePath,
                CreatedAt = i.CreatedAt
            }).ToList();

            return result;
        }


    }
}
