using System.ComponentModel.DataAnnotations;
using RedZone.Common;

namespace RedZone.ViewModels.Prediction
{
    public class PredictionCreateViewModel
    {
        public int MatchId { get; set; }

        public string? HomeTeam { get; set; }

        public string? AwayTeam { get; set; }

        [Required(ErrorMessage = ValidationConstants.Prediction.HomeGoalsRequiredError)]
        [Range(ValidationConstants.Prediction.GoalsMin,
            ValidationConstants.Prediction.GoalsMax,
            ErrorMessage = ValidationConstants.Prediction.GoalsRangeError)]
        public int PredictedHomeGoals { get; set; }

        [Required(ErrorMessage = ValidationConstants.Prediction.AwayGoalsRequiredError)]
        [Range(ValidationConstants.Prediction.GoalsMin,
            ValidationConstants.Prediction.GoalsMax,
            ErrorMessage = ValidationConstants.Prediction.GoalsRangeError)]
        public int PredictedAwayGoals { get; set; }
    }
}