using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Model.Entities;

namespace EZRide_Project.Services
{
    public interface IChatService
    {
        Task<Conversation> GetOrCreateConversationAsync(int participant1Id, int participant2Id, Conversation.ConversationType type);
        Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(int userId);
        Task<IEnumerable<ChatMessageDto>> GetMessagesAsync(int conversationId, int take = 100);
        Task<ChatMessageDto> SaveMessageAsync(ChatMessage message);
        Task MarkConversationMessagesReadAsync(int conversationId, int readerUserId);
        Task<IEnumerable<ConversationWithUserDto>> GetConversationsWithUserDetailsAsync(int userId);
    }
}
