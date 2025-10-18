using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SignalrMvc.Models
{
    public class UserConnection
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ConnectionId { get; set; } = string.Empty;

        public DateTime ConnectedAt { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}
