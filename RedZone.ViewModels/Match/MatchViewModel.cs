using RedZone.Data.Models.Enums;


namespace RedZone.ViewModels.Match
{
    public class MatchIndexViewModel
    {
        public int Id { get; set; }

        public string HomeTeam { get; set; } = null!;

        public string AwayTeam { get; set; } = null!;

        public DateTime MatchDate { get; set; }

        public string CompetitionName { get; set; } = null!;

        public MatchStatus Status { get; set; }

        public bool HasPredicted { get; set; }
    }
}