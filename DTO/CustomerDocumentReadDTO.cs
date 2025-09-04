namespace EZRide_Project.DTO
{
    public class CustomerDocumentReadDTO
    {
        public int DocumentId { get; set; }
        public int UserId { get; set; }
        public string? AgeProofPath { get; set; }
        public string? AddressProofPath { get; set; }
        public string? DLImagePath { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
