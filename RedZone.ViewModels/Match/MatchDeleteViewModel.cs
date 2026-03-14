namespace RedZone.ViewModels.Match
{
    public class MatchDeleteViewModel
    {
        public int Id { get; set; }

        public string HomeTeam { get; set; } = null!;

        public string AwayTeam { get; set; } = null!;

        public DateTime MatchDate { get; set; }

        public string CompetitionName { get; set; } = null!;
    }
}