using System.ComponentModel.DataAnnotations;
using RedZone.Common;
using RedZone.ViewModels.Competition;

namespace RedZone.ViewModels.Match
{
    public class MatchEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = ValidationConstants.Match.TeamNameRequiredError)]
        [StringLength(ValidationConstants.Match.TeamNameMaxLength,
            MinimumLength = ValidationConstants.Match.TeamNameMinLength,
            ErrorMessage = ValidationConstants.Match.TeamNameLengthError)]
        public string HomeTeam { get; set; } = null!;

        [Required(ErrorMessage = ValidationConstants.Match.TeamNameRequiredError)]
        [StringLength(ValidationConstants.Match.TeamNameMaxLength,
            MinimumLength = ValidationConstants.Match.TeamNameMinLength,
            ErrorMessage = ValidationConstants.Match.TeamNameLengthError)]
        public string AwayTeam { get; set; } = null!;

        [Required(ErrorMessage = ValidationConstants.Match.DateRequiredError)]
        public DateTime MatchDate { get; set; }

        [Required(ErrorMessage = ValidationConstants.Match.CompetitionRequiredError)]
        [Range(ValidationConstants.Match.CompetitionIdMin,
            ValidationConstants.Match.CompetitionIdMax,
            ErrorMessage = ValidationConstants.Match.CompetitionInvalidError)]
        public int CompetitionId { get; set; }

        public IEnumerable<CompetitionViewModel> Competitions { get; set; }
            = new List<CompetitionViewModel>();
    }
}