using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AI.CryptoAdvisor.Api.Models
{
    public class ContentCache
    {
        [Key]
        public int Id { get; set; }
        public string ContentType { get; set; }
        public string Data { get; set; }
        public DateTime FetchedAt { get; set; } = DateTime.UtcNow;
    }
}
