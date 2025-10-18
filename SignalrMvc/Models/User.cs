using System.ComponentModel.DataAnnotations;

namespace SignalrMvc.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? FullName { get; set; }

        [MaxLength(255)]
        public string? Email { get; set; }

        [MaxLength(255)]
        public string? ProfileImage { get; set; }

        public bool IsOnline { get; set; } = false;

        public DateTime? LastSeen { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<PrivateMessage> SentMessages { get; set; } = new List<PrivateMessage>();
        public virtual ICollection<PrivateMessage> ReceivedMessages { get; set; } = new List<PrivateMessage>();
        public virtual ICollection<UserConnection> Connections { get; set; } = new List<UserConnection>();
    }
}
