using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalrMvc.Models
{
    public class PrivateMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        public int ReceiverId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime SentAt { get; set; } = DateTime.Now;

        public DateTime? ReadAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(SenderId))]
        public virtual User Sender { get; set; } = null!;

        [ForeignKey(nameof(ReceiverId))]
        public virtual User Receiver { get; set; } = null!;
    }
}
