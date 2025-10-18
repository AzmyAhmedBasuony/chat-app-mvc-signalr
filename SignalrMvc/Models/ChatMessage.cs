using System.ComponentModel.DataAnnotations;

namespace SignalrMvc.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string SenderName { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string? ConnectionId { get; set; }
    }
}
