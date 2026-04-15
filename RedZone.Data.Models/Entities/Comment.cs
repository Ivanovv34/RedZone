using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace RedZone.Data.Models.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 2)]
        public string Content { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int MatchId { get; set; }

        public Match Match { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;

        public IdentityUser User { get; set; } = null!;
    }
}