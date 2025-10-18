using System.ComponentModel.DataAnnotations;

namespace SignalrMvc.DTOs
{
    public class LoginDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? ProfileImage { get; set; }
        public string Token { get; set; } = string.Empty;
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? ProfileImage { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastSeen { get; set; }
    }

    public class SendMessageDto
    {
        [Required]
        public int ReceiverId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Message { get; set; } = string.Empty;
    }

    public class MessageDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; } = string.Empty;
        public string? SenderFullName { get; set; }
        public string? SenderProfileImage { get; set; }
        public int ReceiverId { get; set; }
        public string ReceiverUsername { get; set; } = string.Empty;
        public string? ReceiverFullName { get; set; }
        public string? ReceiverProfileImage { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }

    public class ChatHistoryDto
    {
        public UserDto OtherUser { get; set; } = null!;
        public List<MessageDto> Messages { get; set; } = new();
        public int UnreadCount { get; set; }
    }
}
