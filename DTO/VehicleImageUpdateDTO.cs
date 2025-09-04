namespace EZRide_Project.DTO
{
    public class VehicleImageUpdateDTO
    {

        public int VehicleImageId { get; set; } 
        public IFormFile NewImageFile { get; set; }
    }
}
