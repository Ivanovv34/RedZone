namespace RedZone.ViewModels.Match
{
    public class MatchEditViewModel
    {
        public int Id { get; set; }

        public string HomeTeam { get; set; } = null!;

        public string AwayTeam { get; set; } = null!;

        public DateTime MatchDate { get; set; }

        public int CompetitionId { get; set; }
    }
}