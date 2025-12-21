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

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(dto.ImageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                    return ApiResponseHelper.FileNotAllow("Only JPG, JPEG, PNG, and WEBP are allowed.");

                //  SAFE WebRoot (Local + Render)
                var webRoot = _env.WebRootPath
                    ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                var folder = Path.Combine(webRoot, "Upload_image", "vehicles");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fileName = $"{Guid.NewGuid()}{extension}";
                var fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }

                //  SAVE RELATIVE PATH ONLY
                var vehicleImage = new VehicleImage
                {
                    VehicleId = dto.VehicleId,
                    ImagePath = $"/Upload_image/vehicles/{fileName}",
                    CreatedAt = DateTime.UtcNow
                };

                await _imageRepository.AddVehicleImageAsync(vehicleImage);
                await _imageRepository.SaveChangesAsync();

                return ApiResponseHelper.Success("Vehicle image uploaded successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.ServerError(ex.Message);
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
                var extension = Path.GetExtension(dto.NewImageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                    return ApiResponseHelper.FileNotAllow("Only JPG, JPEG, PNG, and WEBP files are allowed.");

                //  Get existing image
                var existingImage = await _imageRepository.GetVehicleImageByIdAsync(dto.VehicleImageId);
                if (existingImage == null)
                    return ApiResponseHelper.NotFound("Vehicle image");

                //  SAFE WebRoot (Local + Render)
                var webRoot = _env.WebRootPath
                    ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                var folderPath = Path.Combine(webRoot, "Upload_image", "vehicles");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                //  Delete old image (if exists)
                if (!string.IsNullOrEmpty(existingImage.ImagePath))
                {
                    var oldImageFullPath = Path.Combine(
                        webRoot,
                        existingImage.ImagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
                    );

                    if (File.Exists(oldImageFullPath))
                        File.Delete(oldImageFullPath);
                }

                //  Save new image
                var newFileName = $"{Guid.NewGuid()}{extension}";
                var newFullPath = Path.Combine(folderPath, newFileName);

                using (var stream = new FileStream(newFullPath, FileMode.Create))
                {
                    await dto.NewImageFile.CopyToAsync(stream);
                }

                //  Update DB (SAVE RELATIVE PATH ONLY)
                existingImage.ImagePath = $"/Upload_image/vehicles/{newFileName}";
                existingImage.CreatedAt = DateTime.UtcNow;

                _imageRepository.UpdateVehicleImage(existingImage);
                await _imageRepository.SaveChangesAsync();

                return ApiResponseHelper.Success("Vehicle image updated successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.ServerError(ex.Message);
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

                //  SAFE WebRoot (Local + Render)
                var webRoot = _env.WebRootPath
                    ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                // Build full file path safely
                if (!string.IsNullOrEmpty(image.ImagePath))
                {
                    var fullPath = Path.Combine(
                        webRoot,
                        image.ImagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
                    );

                    if (File.Exists(fullPath))
                        File.Delete(fullPath);
                }

                //  Delete DB record
                _imageRepository.DeleteVehicleImage(image);
                await _imageRepository.SaveChangesAsync();

                return ApiResponseHelper.Success("Vehicle image deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponseHelper.ServerError(ex.Message);
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
