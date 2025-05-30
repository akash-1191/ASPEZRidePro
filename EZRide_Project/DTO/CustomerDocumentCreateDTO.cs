namespace EZRide_Project.DTO
{
    public class CustomerDocumentCreateDTO
    {
        public int UserId { get; set; }
        public IFormFile? AgeProof { get; set; }
        public IFormFile? AddressProof { get; set; }
        public IFormFile? DLImage { get; set; }
        public string Status { get; set; }
    }
}
