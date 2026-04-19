using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using RedZone.Common;

namespace RedZone.Data.Models.Entities
{
    public class Prediction
    {
        public int Id { get; set; }

        [Range(ValidationConstants.PredictionEntity.GoalsMin,
            ValidationConstants.PredictionEntity.GoalsMax)]
        public int PredictedHomeGoals { get; set; }

        [Range(ValidationConstants.PredictionEntity.GoalsMin,
            ValidationConstants.PredictionEntity.GoalsMax)]
        public int PredictedAwayGoals { get; set; }

        public int? PointsEarned { get; set; }

        public bool IsCalculated { get; set; }

        public int MatchId { get; set; }

        public Match Match { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;

        public IdentityUser User { get; set; } = null!;
    }
}