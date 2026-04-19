using System.ComponentModel.DataAnnotations;
using RedZone.Common;

namespace RedZone.Data.Models.Entities
{
    public class MatchResult
    {
        public int Id { get; set; }

        [Range(ValidationConstants.MatchResult.GoalsMin,
            ValidationConstants.MatchResult.GoalsMax)]
        public int HomeGoals { get; set; }

        [Range(ValidationConstants.MatchResult.GoalsMin,
            ValidationConstants.MatchResult.GoalsMax)]
        public int AwayGoals { get; set; }

        public DateTime EnteredAt { get; set; } = DateTime.UtcNow;

        public int MatchId { get; set; }

        public Match Match { get; set; } = null!;
    }
}