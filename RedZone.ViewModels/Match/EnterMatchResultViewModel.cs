using System.ComponentModel.DataAnnotations;
using RedZone.Common;

namespace RedZone.ViewModels.Match
{
    public class EnterMatchResultViewModel
    {
        public int MatchId { get; set; }

        public string HomeTeam { get; set; } = null!;

        public string AwayTeam { get; set; } = null!;

        [Required(ErrorMessage = ValidationConstants.MatchResult.HomeGoalsRequiredError)]
        [Range(ValidationConstants.MatchResult.GoalsMin,
            ValidationConstants.MatchResult.GoalsMax,
            ErrorMessage = ValidationConstants.MatchResult.GoalsRangeError)]
        [Display(Name = "Home Goals")]
        public int HomeGoals { get; set; }

        [Required(ErrorMessage = ValidationConstants.MatchResult.AwayGoalsRequiredError)]
        [Range(ValidationConstants.MatchResult.GoalsMin,
            ValidationConstants.MatchResult.GoalsMax,
            ErrorMessage = ValidationConstants.MatchResult.GoalsRangeError)]
        [Display(Name = "Away Goals")]
        public int AwayGoals { get; set; }
    }
}