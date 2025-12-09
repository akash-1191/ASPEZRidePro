namespace EZRide_Project.DTO.Vehile_Owner_DTo
{
    public class ConversationDto
    {
        public int ConversationId { get; set; }
        public int Participant1Id { get; set; }
        public int Participant2Id { get; set; }
        public string Participant1Name { get; set; }
        public string Participant2Name { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public LastMessageDto LastMessage { get; set; }
        public int UnreadCount { get; set; }
    }

    public class ConversationWithUserDto
    {
        public int ConversationId { get; set; }
        public int OtherUserId { get; set; }
        public string OtherUserName { get; set; }
        public string OtherUserRole { get; set; }
        public string OtherUserEmail { get; set; }
        public LastMessageDto LastMessage { get; set; }
        public int UnreadCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class LastMessageDto
    {
        public int MessageId { get; set; }
        public string MessageText { get; set; }
        public int SenderId { get; set; }
        public bool IsSentByMe { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
    }

    public class ChatMessageDto
    {
        public int MessageId { get; set; }
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderRole { get; set; }
        public string MessageText { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
        public bool IsEdited { get; set; }
    }

    public class SendMessageRequestDto
    {
        public int ConversationId { get; set; }
        public string MessageText { get; set; }
    }

    public class CreateConversationRequestDto
    {
        public int ParticipantId { get; set; }
        public string Type { get; set; } = "AdminOwner"; // Default
    }
}
