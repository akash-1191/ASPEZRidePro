using EZRide_Project.DTO.Vehile_Owner_DTo;
using EZRide_Project.Model.Entities;
using EZRide_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EZRide_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Customer,OwnerVehicle,Driver")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst("UserId")?.Value;
            return claim != null ? int.Parse(claim) : 0;
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0) return Unauthorized();

                var conv = await _chatService.GetUserConversationsAsync(userId);
                return Ok(conv);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting conversations");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("conversations-with-users")]
        public async Task<IActionResult> GetConversationsWithUsers()
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0) return Unauthorized();

                var conv = await _chatService.GetConversationsWithUserDetailsAsync(userId);
                return Ok(conv);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting conversations with users");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("conversation")]
        public async Task<IActionResult> CreateOrGetConversation([FromBody] CreateConversationRequestDto req)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0) return Unauthorized();

                // Parse conversation type
                if (!Enum.TryParse<Conversation.ConversationType>(req.Type, true, out var convType))
                    convType = Conversation.ConversationType.AdminOwner;

                var conv = await _chatService.GetOrCreateConversationAsync(userId, req.ParticipantId, convType);
                return Ok(conv);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating/getting conversation");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{conversationId}/messages")]
        public async Task<IActionResult> GetMessages(int conversationId, [FromQuery] int take = 100)
        {
            try
            {
                var messages = await _chatService.GetMessagesAsync(conversationId, take);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting messages for conversation {conversationId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{conversationId}/send")]
        public async Task<IActionResult> SendMessage(int conversationId, [FromBody] SendMessageRequestDto req)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0) return Unauthorized();

                var message = new ChatMessage
                {
                    ConversationId = conversationId,
                    SenderId = userId,
                    MessageText = req.MessageText,
                    Timestamp = DateTime.UtcNow,
                    Status = ChatMessage.MessageStatus.Sent
                };

                var saved = await _chatService.SaveMessageAsync(message);
                return Ok(saved);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending message to conversation {conversationId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{conversationId}/mark-read")]
        public async Task<IActionResult> MarkMessagesRead(int conversationId)
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0) return Unauthorized();

                await _chatService.MarkConversationMessagesReadAsync(conversationId, userId);
                return Ok(new { message = "Messages marked as read" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking messages as read for conversation {conversationId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                int userId = GetUserId();
                if (userId == 0) return Unauthorized();

                // Note: You need to add GetUnreadMessageCountAsync method in service
                // For now returning 0
                return Ok(new { unreadCount = 0 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count");
                return StatusCode(500, "Internal server error");
            }
        }





    }
}