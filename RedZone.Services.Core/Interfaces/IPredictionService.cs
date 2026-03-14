using RedZone.ViewModels.Prediction;

namespace RedZone.Services.Core.Interfaces
{
    public interface IPredictionService
    {
        Task AddAsync(PredictionCreateViewModel model, string userId);

        Task<IEnumerable<PredictionViewModel>> GetUserPredictionsAsync(string userId);
    }
}