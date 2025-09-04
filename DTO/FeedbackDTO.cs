namespace EZRide_Project.DTO
{
    public class FeedbackDTO
    {
        public int? UserId { get; set; }
        public string? FeedbackType { get; set; }  
        public string? Message { get; set; }

        public int? FeedbackId { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? FullName { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }

    }
}
