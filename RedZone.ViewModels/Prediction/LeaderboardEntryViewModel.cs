namespace RedZone.ViewModels.Prediction
{
    public class LeaderboardEntryViewModel
    {
        public int Position { get; set; }

        public string UserId { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public int TotalPoints { get; set; }

        public int TotalPredictions { get; set; }

        public int ExactScores { get; set; }

        public int CorrectResults { get; set; }
    }
}