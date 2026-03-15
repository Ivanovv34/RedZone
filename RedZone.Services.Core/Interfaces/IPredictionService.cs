using RedZone.ViewModels.Prediction;

namespace RedZone.Services.Core.Interfaces
{
    public interface IPredictionService
    {
        Task<PredictionCreateViewModel?> GetPredictionFormAsync(int matchId);

        Task CreateAsync(PredictionCreateViewModel model, string userId);

        Task<IEnumerable<PredictionViewModel>> GetUserPredictionsAsync(string userId);
    }
}