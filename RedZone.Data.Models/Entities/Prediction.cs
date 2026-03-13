using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace RedZone.Data.Models.Entities
{
    public class Prediction
    {
        public int Id { get; set; }

        public int PredictedHomeGoals { get; set; }

        public int PredictedAwayGoals { get; set; }

        public int MatchId { get; set; }

        public Match Match { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;

        public IdentityUser User { get; set; } = null!;
    }
}