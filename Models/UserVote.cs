using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI.CryptoAdvisor.Api.Models
{
    public class UserVote
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        public string CardType { get; set; }
        public bool Vote { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
