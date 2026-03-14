using System.ComponentModel.DataAnnotations;
using RedZone.ViewModels.Competition;

namespace RedZone.ViewModels.Match
{
    public class MatchEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string HomeTeam { get; set; } = null!;

        [Required]
        public string AwayTeam { get; set; } = null!;

        [Required]
        public DateTime MatchDate { get; set; }

        [Required]
        public int CompetitionId { get; set; }

        public IEnumerable<CompetitionViewModel> Competitions { get; set; }
            = new List<CompetitionViewModel>();
    }
}