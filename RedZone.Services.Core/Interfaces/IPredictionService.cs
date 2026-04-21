using RedZone.ViewModels.Prediction;

namespace RedZone.Services.Core.Interfaces
{
    public interface IPredictionService
    {
        Task<PredictionCreateViewModel?> GetPredictionFormAsync(int matchId);

        Task<bool> CreateAsync(PredictionCreateViewModel model, string userId);

        Task<IEnumerable<PredictionViewModel>> GetUserPredictionsAsync(string userId);

        Task<PredictionMineViewModel> GetUserPredictionsPagedAsync(string userId, int page = 1, int pageSize = 10);

        Task<bool> HasUserPredictedAsync(int matchId, string userId);

        Task<bool> DeleteAsync(int predictionId, string userId);

        Task CalculatePointsAsync(int matchId);

        Task<UserStatsViewModel> GetUserStatsAsync(string userId);

        Task<IEnumerable<LeaderboardEntryViewModel>> GetLeaderboardAsync();
    }
}