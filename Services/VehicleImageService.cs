using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EZRide_Project.DTO;
using EZRide_Project.Helpers;
using EZRide_Project.Model;
using EZRide_Project.Model.Entities;
using EZRide_Project.Repositories;

namespace EZRide_Project.Services
{
    public class VehicleImageService : IVehicleImageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly IVehicleImageRepository _imageRepository;

        public VehicleImageService(IVehicleImageRepository imageRepository, Cloudinary cloudinary)
        {
            _imageRepository = imageRepository;
            _cloudinary = cloudinary;
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

                using var stream = dto.ImageFile.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(dto.ImageFile.FileName, stream),
                    Folder = "EZRide/Vehicles",  // Cloudinary folder
                    Transformation = new Transformation()
                        .Quality("auto") // auto optimize
                        .FetchFormat("auto")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                    return ApiResponseHelper.Fail(uploadResult.Error.Message);

                var vehicleImage = new VehicleImage
                {
                    VehicleId = dto.VehicleId,
                    ImagePath = uploadResult.SecureUrl.AbsoluteUri, // final URL
                    PublicId = uploadResult.PublicId,              // required for delete/update
                    CreatedAt = DateTime.UtcNow
                };

                await _imageRepository.AddVehicleImageAsync(vehicleImage);
                await _imageRepository.SaveChangesAsync();

                return ApiResponseHelper.Success("Vehicle image uploaded successfully!");
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
                    return ApiResponseHelper.FileNotAllow("Only JPG, JPEG, PNG, WEBP allowed.");

                var existingImage = await _imageRepository.GetVehicleImageByIdAsync(dto.VehicleImageId);
                if (existingImage == null)
                    return ApiResponseHelper.NotFound("Vehicle image not found.");

                // DELETE OLD IMAGE from cloudinary
                if (!string.IsNullOrEmpty(existingImage.PublicId))
                {
                    var delParams = new DeletionParams(existingImage.PublicId);
                    await _cloudinary.DestroyAsync(delParams);
                }

                //  UPLOAD NEW IMAGE TO CLOUDINARY
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(dto.NewImageFile.FileName, dto.NewImageFile.OpenReadStream()),
                    Folder = "EZRide/Vehicles"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                //  Update Database
                existingImage.ImagePath = uploadResult.SecureUrl.ToString();
                existingImage.PublicId = uploadResult.PublicId;
                existingImage.CreatedAt = DateTime.UtcNow;

                _imageRepository.UpdateVehicleImage(existingImage);
                await _imageRepository.SaveChangesAsync();

                return ApiResponseHelper.Success("Vehicle image updated successfully!");
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
                    return ApiResponseHelper.NotFound("Vehicle image not found");

                //  DELETE from Cloudinary
                if (!string.IsNullOrEmpty(image.PublicId))
                {
                    var delParams = new DeletionParams(image.PublicId);
                    await _cloudinary.DestroyAsync(delParams);
                }

                _imageRepository.DeleteVehicleImage(image);
                await _imageRepository.SaveChangesAsync();

                return ApiResponseHelper.Success("Vehicle image deleted successfully!");
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

            return images.Select(i => new VehicleImageResponseDTO
            {
                VehicleImageId = i.VehicleImageId,
                VehicleId = i.VehicleId,
                ImagePath = i.ImagePath, // Cloud URL
                PublicId = i.PublicId ?? string.Empty,
                CreatedAt = i.CreatedAt
            }).ToList();
        }



    }
}
