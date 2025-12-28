namespace EZRide_Project.DTO
{
    public class VehicleImageResponseDTO
    {
        public int VehicleImageId { get; set; }
        public int VehicleId { get; set; }
        public string ImagePath { get; set; }
        public string? PublicId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
