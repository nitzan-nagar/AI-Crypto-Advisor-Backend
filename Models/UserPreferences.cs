using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AI.CryptoAdvisor.Api.Models.Enums;

namespace AI.CryptoAdvisor.Api.Models
{
    public class UserPreferences
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public InvestorType InvestorType { get; set; }
        public List<string> PreferredAssets { get; set; } = new List<string>();

        public List<string> ContentTypes { get; set; } = new List<string>();
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
