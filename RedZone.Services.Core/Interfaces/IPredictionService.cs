using RedZone.ViewModels.Prediction;

namespace RedZone.Services.Core.Interfaces
{
    public interface IPredictionService
    {
        Task<PredictionCreateViewModel?> GetPredictionFormAsync(int matchId);

        Task CreateAsync(PredictionCreateViewModel model, string userId);

        Task<IEnumerable<PredictionViewModel>> GetUserPredictionsAsync(string userId);

        Task<bool> HasUserPredictedAsync(int matchId, string userId);

        Task<bool> DeleteAsync(int predictionId, string userId);

        Task CalculatePointsAsync(int matchId);

        Task<UserStatsViewModel> GetUserStatsAsync(string userId);

        Task<IEnumerable<LeaderboardEntryViewModel>> GetLeaderboardAsync();
    }
}