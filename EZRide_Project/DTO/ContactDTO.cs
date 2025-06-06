namespace EZRide_Project.DTO
{
    public class ContactDTO
    {
        //public int? UserId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }   
        public string Status { get; set; } // Use string for enum mapping

        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }

    }
}
