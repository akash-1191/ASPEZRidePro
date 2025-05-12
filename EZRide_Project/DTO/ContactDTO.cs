namespace EZRide_Project.DTO
{
    public class ContactDTO
    {
        public int ContactId { get; set; }
        public int UserId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }  // Enum as string
        public DateTime CreatedAt { get; set; }
    }
}
