namespace RedZone.ViewModels.Match
{
    public class MatchCreateViewModel
    {
        public string HomeTeam { get; set; } = null!;

        public string AwayTeam { get; set; } = null!;

        public DateTime MatchDate { get; set; }

        public int CompetitionId { get; set; }
    }
}