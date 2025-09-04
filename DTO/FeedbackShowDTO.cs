namespace EZRide_Project.DTO
{
    public class FeedbackShowDTO
    {
      
            public int FeedbackId { get; set; }
            public int UserId { get; set; }
            public string FeedbackType { get; set; }
            public string Message { get; set; }
            public string Status { get; set; }
            public DateTime CreatedAt { get; set; }

            public UserDTO User { get; set; }
        }

        public class UserDTO
        {
            public int UserId { get; set; }
            public string Firstname { get; set; }
            public string? Middlename { get; set; }
            public string Lastname { get; set; }
            public int Age { get; set; }
            public string Gender { get; set; }
            public string Image { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string Status { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
