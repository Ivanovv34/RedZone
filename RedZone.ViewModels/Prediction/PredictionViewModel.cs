namespace RedZone.ViewModels.Prediction
{
    public class PredictionViewModel
    {
        public int Id { get; set; }

        public int MatchId { get; set; }

        public string HomeTeam { get; set; } = null!;

        public string AwayTeam { get; set; } = null!;

        public DateTime MatchDate { get; set; }

        public int PredictedHomeGoals { get; set; }

        public int PredictedAwayGoals { get; set; }

        public int? PointsEarned { get; set; }

        public bool IsCalculated { get; set; }

        public int? ActualHomeGoals { get; set; }

        public int? ActualAwayGoals { get; set; }
    }
}