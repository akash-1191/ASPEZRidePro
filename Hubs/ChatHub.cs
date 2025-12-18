using EZRide_Project.Data;
using EZRide_Project.Model.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EZRide_Project.Hubs
{
    [Authorize(Roles = "Admin,Customer,OwnerVehicle,Driver")]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ApplicationDbContext db, ILogger<ChatHub> logger)
        {
            _db = db;
            _logger = logger;
        }

        private int GetCurrentUserId()
        {
            var claim = Context.User?.FindFirst("UserId")?.Value;
            if (int.TryParse(claim, out int userId) && userId > 0)
                return userId;

            throw new HubException("Invalid UserId claim");
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                int userId = GetCurrentUserId();
                // Add connection to user-specific group for targeted messaging
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");

                _logger.LogInformation($"User {userId} connected with connection ID: {Context.ConnectionId}");
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in OnConnectedAsync for connection: {Context.ConnectionId}");
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                if (exception != null)
                    _logger.LogError(exception, $"Disconnected with error: {Context.ConnectionId}");
                else
                    _logger.LogInformation($"User disconnected: {Context.ConnectionId}");

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnDisconnectedAsync");
            }
        }

        // Join conversation group with validation
        public async Task JoinConversation(int conversationId)
        {
            try
            {
                int userId = GetCurrentUserId();

                // Validate user is participant of this conversation
                var conversation = await _db.Conversations
                    .FirstOrDefaultAsync(c => c.ConversationId == conversationId &&
                        (c.Participant1Id == userId || c.Participant2Id == userId));

                if (conversation == null)
                    throw new HubException("You are not a participant of this conversation");

                var groupName = $"conversation_{conversationId}";
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                // Mark messages as delivered
                var undeliveredMessages = await _db.ChatMessages
                    .Where(m => m.ConversationId == conversationId &&
                                m.SenderId != userId &&
                                m.Status == ChatMessage.MessageStatus.Sent)
                    .ToListAsync();

                if (undeliveredMessages.Any())
                {
                    foreach (var message in undeliveredMessages)
                        message.Status = ChatMessage.MessageStatus.Delivered;

                    await _db.SaveChangesAsync();
                    await Clients.Group(groupName).SendAsync("MessagesDelivered", conversationId);
                }

                _logger.LogInformation($"User {userId} joined conversation {conversationId}");
            }
            catch (HubException hex)
            {
                _logger.LogWarning(hex, $"HubException in JoinConversation for user {Context.User?.Identity?.Name}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in JoinConversation for connection: {Context.ConnectionId}");
                throw new HubException("Error joining conversation");
            }
        }

        public async Task LeaveConversation(int conversationId)
        {
            try
            {
                var groupName = $"conversation_{conversationId}";
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                _logger.LogInformation($"Connection {Context.ConnectionId} left conversation {conversationId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in LeaveConversation for connection: {Context.ConnectionId}");
            }
        }

        // Send message: saves to DB and broadcasts to group
        public async Task SendMessage(int conversationId, string messageText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(messageText))
                    throw new HubException("Message cannot be empty");

                if (messageText.Length > 5000)
                    throw new HubException("Message is too long");

                int senderId = GetCurrentUserId();

                // Validate conversation and user participation
                var conversation = await _db.Conversations
                    .Include(c => c.Participant1)
                    .Include(c => c.Participant2)
                    .FirstOrDefaultAsync(c => c.ConversationId == conversationId &&
                        (c.Participant1Id == senderId || c.Participant2Id == senderId));

                if (conversation == null)
                    throw new HubException("You are not a participant of this conversation");

                if (conversation.Status == Conversation.ConversationStatus.Closed)
                    throw new HubException("This conversation is closed");

                // Create and save message
                var chatMessage = new ChatMessage
                {
                    ConversationId = conversationId,
                    SenderId = senderId,
                    MessageText = messageText.Trim(),
                    Timestamp = DateTime.UtcNow,
                    Status = ChatMessage.MessageStatus.Sent
                };

                _db.ChatMessages.Add(chatMessage);
                await _db.SaveChangesAsync();

                // Prepare DTO with sender info
                //var dto = new
                //{
                //    MessageId = chatMessage.MessageId,
                //    ConversationId = chatMessage.ConversationId,
                //    SenderId = chatMessage.SenderId,
                //    SenderName = $"{conversation.Participant1?.Firstname} {conversation.Participant1?.Lastname}",
                //    SenderRole = conversation.Participant1?.Role?.RoleName.ToString() ?? "Unknown",
                //    MessageText = chatMessage.MessageText,
                //    Timestamp = chatMessage.Timestamp,
                //    Status = chatMessage.Status.ToString()
                //};
                var sender =
    conversation.Participant1Id == senderId
        ? conversation.Participant1
        : conversation.Participant2;

                var dto = new
                {
                    MessageId = chatMessage.MessageId,
                    ConversationId = chatMessage.ConversationId,
                    SenderId = chatMessage.SenderId,
                    SenderName = $"{conversation.Participant1?.Firstname} {conversation.Participant1?.Lastname}",
                    SenderRole = conversation.Participant1?.Role?.RoleName.ToString() ?? "Unknown",
                    MessageText = chatMessage.MessageText,
                    Timestamp = chatMessage.Timestamp,
                    Status = chatMessage.Status.ToString()
                };

                // Broadcast to conversation group
                var groupName = $"conversation_{conversationId}";
                await Clients.Group(groupName).SendAsync("ReceiveMessage", dto);

                // Also send to sender (for confirmation) and target user if online
                var receiverId = conversation.Participant1Id == senderId ? conversation.Participant2Id : conversation.Participant1Id;
                await Clients.Group($"user_{senderId}").SendAsync("MessageSent", dto);

                _logger.LogInformation($"User {senderId} sent message in conversation {conversationId}");
            }
            catch (HubException hex)
            {
                _logger.LogWarning(hex, $"HubException in SendMessage for user {Context.User?.Identity?.Name}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in SendMessage for connection: {Context.ConnectionId}");
                throw new HubException("Error sending message");
            }
        }

        // Mark messages as read (when user opens the chat window)
        public async Task MarkMessagesRead(int conversationId)
        {
            try
            {
                int userId = GetCurrentUserId();

                // Validate user is participant
                var isParticipant = await _db.Conversations
                    .AnyAsync(c => c.ConversationId == conversationId &&
                        (c.Participant1Id == userId || c.Participant2Id == userId));

                if (!isParticipant)
                    throw new HubException("You are not a participant of this conversation");

                var messagesToMark = await _db.ChatMessages
                    .Where(m => m.ConversationId == conversationId &&
                                m.SenderId != userId &&
                                m.Status != ChatMessage.MessageStatus.Read)
                    .ToListAsync();

                if (messagesToMark.Any())
                {
                    foreach (var message in messagesToMark)
                        message.Status = ChatMessage.MessageStatus.Read;

                    await _db.SaveChangesAsync();

                    var groupName = $"conversation_{conversationId}";
                    await Clients.Group(groupName).SendAsync("MessagesRead", new
                    {
                        ConversationId = conversationId,
                        ReadByUserId = userId,
                        ReadAt = DateTime.UtcNow
                    });

                    _logger.LogInformation($"User {userId} marked messages as read in conversation {conversationId}");
                }
            }
            catch (HubException hex)
            {
                _logger.LogWarning(hex, $"HubException in MarkMessagesRead for user {Context.User?.Identity?.Name}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in MarkMessagesRead for connection: {Context.ConnectionId}");
                throw new HubException("Error marking messages as read");
            }
        }
    }
}

