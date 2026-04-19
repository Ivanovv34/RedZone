namespace RedZone.ViewModels.Prediction
{
    public class PredictionMineViewModel
    {
        public IEnumerable<PredictionViewModel> Predictions { get; set; }
            = new List<PredictionViewModel>();

        public UserStatsViewModel Stats { get; set; }
            = new UserStatsViewModel();
    }
}