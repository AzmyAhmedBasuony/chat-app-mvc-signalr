using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SignalrMvc.DTOs;
using SignalrMvc.Services;

namespace SignalrMvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IChatService _chatService;

        public MessagesController(IChatService chatService)
        {
            _chatService = chatService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                throw new UnauthorizedAccessException();
            return userId;
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<UserDto>>> GetUsers()
        {
            var currentUserId = GetCurrentUserId();
            var users = await _chatService.GetAllUsersAsync(currentUserId);
            return Ok(users);
        }

        [HttpGet("history/{otherUserId}")]
        public async Task<ActionResult<ChatHistoryDto>> GetChatHistory(int otherUserId)
        {
            var currentUserId = GetCurrentUserId();
            var history = await _chatService.GetChatHistoryAsync(currentUserId, otherUserId);
            return Ok(history);
        }

        [HttpPost("send")]
        public async Task<ActionResult<MessageDto>> SendMessage([FromBody] SendMessageDto dto)
        {
            var currentUserId = GetCurrentUserId();
            var message = await _chatService.SendMessageAsync(currentUserId, dto);
            return Ok(message);
        }

        [HttpPost("mark-read/{senderId}")]
        public async Task<ActionResult> MarkAsRead(int senderId)
        {
            var currentUserId = GetCurrentUserId();
            await _chatService.MarkMessagesAsReadAsync(currentUserId, senderId);
            return Ok();
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var currentUserId = GetCurrentUserId();
            var count = await _chatService.GetUnreadCountAsync(currentUserId);
            return Ok(count);
        }

        [HttpGet("unread-counts-per-user")]
        public async Task<ActionResult<Dictionary<int, int>>> GetUnreadCountsPerUser()
        {
            var currentUserId = GetCurrentUserId();
            var counts = await _chatService.GetUnreadCountsPerUserAsync(currentUserId);
            return Ok(counts);
        }
    }
}
