using System.ComponentModel.DataAnnotations;

namespace RedZone.Data.Models.Entities
{
    public class MatchResult
    {
        public int Id { get; set; }

        [Range(0, 50)]
        public int HomeGoals { get; set; }

        [Range(0, 50)]
        public int AwayGoals { get; set; }

        public DateTime EnteredAt { get; set; } = DateTime.UtcNow;

        public int MatchId { get; set; }

        public Match Match { get; set; } = null!;
    }
}