using EZRide_Project.Data;
using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Services
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _db;
        public ChatService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Conversation> GetOrCreateConversationAsync(int participant1Id, int participant2Id, Conversation.ConversationType type)
        {
            // Check if conversation already exists
            var conv = await _db.Conversations
                .FirstOrDefaultAsync(c =>
   ((c.Participant1Id == participant1Id && c.Participant2Id == participant2Id) ||
    (c.Participant1Id == participant2Id && c.Participant2Id == participant1Id))
    && c.Type == type);

            if (conv != null) return conv;

            // Create new conversation
            var newConv = new Conversation
            {
                Participant1Id = participant1Id,
                Participant2Id = participant2Id,
                Type = type,
                Status = Conversation.ConversationStatus.Open,
                CreatedAt = DateTime.UtcNow
            };

            _db.Conversations.Add(newConv);
            await _db.SaveChangesAsync();

            // Load with participants for immediate use
            return await _db.Conversations
                .Include(c => c.Participant1)
                .Include(c => c.Participant2)
                .FirstOrDefaultAsync(c => c.ConversationId == newConv.ConversationId);
        }

        public async Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(int userId)
        {
            var conversations = await _db.Conversations
                .Where(c => c.Participant1Id == userId || c.Participant2Id == userId)
                .Include(c => c.Participant1)
                .Include(c => c.Participant2)
                .Include(c => c.ChatMessages.OrderByDescending(m => m.Timestamp).Take(1))
                .OrderByDescending(c => c.ChatMessages.Max(m => (DateTime?)m.Timestamp))
                .ToListAsync();

            return conversations.Select(c => new ConversationDto
            {
                ConversationId = c.ConversationId,
                Participant1Id = c.Participant1Id,
                Participant2Id = c.Participant2Id,
                Participant1Name = $"{c.Participant1?.Firstname} {c.Participant1?.Lastname}".Trim(),
                Participant2Name = $"{c.Participant2?.Firstname} {c.Participant2?.Lastname}".Trim(),
                Type = c.Type.ToString(),
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                LastMessage = c.ChatMessages.FirstOrDefault() != null ? new LastMessageDto
                {
                    MessageId = c.ChatMessages.First().MessageId,
                    MessageText = c.ChatMessages.First().MessageText,
                    SenderId = c.ChatMessages.First().SenderId,
                    Timestamp = c.ChatMessages.First().Timestamp,
                    Status = c.ChatMessages.First().Status.ToString()
                } : null,
                UnreadCount = c.ChatMessages.Count(m => m.Status == ChatMessage.MessageStatus.Sent ||
                                                      m.Status == ChatMessage.MessageStatus.Delivered)
            }).ToList();
        }

        public async Task<IEnumerable<ConversationWithUserDto>> GetConversationsWithUserDetailsAsync(int userId)
        {
            var conversations = await _db.Conversations
                .Where(c => c.Participant1Id == userId || c.Participant2Id == userId)
                .Include(c => c.Participant1).ThenInclude(p => p.Role)
                .Include(c => c.Participant2).ThenInclude(p => p.Role)
                .Include(c => c.ChatMessages.OrderByDescending(m => m.Timestamp).Take(10))
                .ToListAsync();

            return conversations.Select(c =>
            {
                var otherUser = c.Participant1Id == userId ? c.Participant2 : c.Participant1;

                return new ConversationWithUserDto
                {
                    ConversationId = c.ConversationId,
                    OtherUserId = otherUser.UserId,
                    OtherUserName = $"{otherUser.Firstname} {otherUser.Lastname}",
                    OtherUserRole = otherUser.Role?.RoleName.ToString() ?? "Unknown",
                    OtherUserEmail = otherUser.Email,
                    LastMessage = c.ChatMessages.FirstOrDefault() != null ? new LastMessageDto
                    {
                        MessageId = c.ChatMessages.First().MessageId,
                        MessageText = c.ChatMessages.First().MessageText,
                        SenderId = c.ChatMessages.First().SenderId,
                        IsSentByMe = c.ChatMessages.First().SenderId == userId,
                        Timestamp = c.ChatMessages.First().Timestamp,
                        Status = c.ChatMessages.First().Status.ToString()
                    } : null,
                    UnreadCount = c.ChatMessages.Count(m =>
                        (m.Status == ChatMessage.MessageStatus.Sent ||
                         m.Status == ChatMessage.MessageStatus.Delivered) &&
                        m.SenderId != userId),
                    CreatedAt = c.CreatedAt
                };
            }).OrderByDescending(c => c.LastMessage?.Timestamp ?? c.CreatedAt).ToList();
        }

        public async Task<IEnumerable<ChatMessageDto>> GetMessagesAsync(int conversationId, int take = 100)
        {
            var messages = await _db.ChatMessages
                .Where(m => m.ConversationId == conversationId)
                .Include(m => m.Sender).ThenInclude(s => s.Role)
                .OrderBy(m => m.Timestamp)
                .Take(take)
                .ToListAsync();

            return messages.Select(m => new ChatMessageDto  
            {
                MessageId = m.MessageId,
                ConversationId = m.ConversationId,
                SenderId = m.SenderId,
                SenderName = $"{m.Sender?.Firstname} {m.Sender?.Lastname}".Trim(),
                SenderRole = m.Sender?.Role?.RoleName.ToString() ?? "Unknown",
                MessageText = m.MessageText,
                Timestamp = m.Timestamp,
                Status = m.Status.ToString(),
                IsEdited = false // Can add edit functionality later
            }).ToList();
        }

        public async Task<ChatMessageDto> SaveMessageAsync(ChatMessage message)
        {
            _db.ChatMessages.Add(message);
            await _db.SaveChangesAsync();

            // Reload with sender details
            var savedMessage = await _db.ChatMessages
                .Include(m => m.Sender).ThenInclude(s => s.Role)
                .FirstOrDefaultAsync(m => m.MessageId == message.MessageId);

            return new ChatMessageDto
            {
                MessageId = savedMessage.MessageId,
                ConversationId = savedMessage.ConversationId,
                SenderId = savedMessage.SenderId,
                SenderName = $"{savedMessage.Sender?.Firstname} {savedMessage.Sender?.Lastname}".Trim(),
                SenderRole = savedMessage.Sender?.Role?.RoleName.ToString() ?? "Unknown",
                MessageText = savedMessage.MessageText,
                Timestamp = savedMessage.Timestamp,
                Status = savedMessage.Status.ToString()
            };
        }

        public async Task MarkConversationMessagesReadAsync(int conversationId, int readerUserId)
        {
            var messagesToMark = await _db.ChatMessages
                .Where(m => m.ConversationId == conversationId &&
                           m.SenderId != readerUserId &&
                           m.Status != ChatMessage.MessageStatus.Read)
                .ToListAsync();

            if (!messagesToMark.Any()) return;

            foreach (var message in messagesToMark)
                message.Status = ChatMessage.MessageStatus.Read;

            await _db.SaveChangesAsync();
        }

        public async Task<int> GetUnreadMessageCountAsync(int userId)
        {
            //return await _db.ChatMessages
            //.CountAsync(m => m.Conversation.Participant1Id == userId ||
            //m.Conversation.Participant2Id == userId &&
            //m.SenderId != userId &&
            //m.Status != ChatMessage.MessageStatus.Read);

            return await _db.ChatMessages.CountAsync(m =>
   (m.Conversation.Participant1Id == userId ||
    m.Conversation.Participant2Id == userId) &&
    m.SenderId != userId &&
    m.Status != ChatMessage.MessageStatus.Read
);
        }
    }
}