using System.ComponentModel.DataAnnotations;

namespace RedZone.ViewModels.Prediction
{
    public class PredictionCreateViewModel
    {
        public int MatchId { get; set; }

        public string? HomeTeam { get; set; }

        public string? AwayTeam { get; set; }

        [Required]
        [Range(0, 20)]
        public int PredictedHomeGoals { get; set; }

        [Required]
        [Range(0, 20)]
        public int PredictedAwayGoals { get; set; }
    }
}