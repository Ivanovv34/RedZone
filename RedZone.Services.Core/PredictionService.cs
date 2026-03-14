using Microsoft.EntityFrameworkCore;
using RedZone.Data;
using RedZone.Data.Models.Entities;
using RedZone.Services.Core.Interfaces;
using RedZone.ViewModels.Prediction;

namespace RedZone.Services.Core
{
    public class PredictionService : IPredictionService
    {
        private readonly RedZoneDbContext context;

        public PredictionService(RedZoneDbContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(PredictionCreateViewModel model, string userId)
        {
            var prediction = new Prediction
            {
                PredictedHomeGoals = model.PredictedHomeGoals,
                PredictedAwayGoals = model.PredictedAwayGoals,
                MatchId = model.MatchId,
                UserId = userId
            };

            await context.Predictions.AddAsync(prediction);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PredictionViewModel>> GetUserPredictionsAsync(string userId)
        {
            return await context.Predictions
                .Include(p => p.Match)
                .Where(p => p.UserId == userId)
                .Select(p => new PredictionViewModel
                {
                    Id = p.Id,
                    PredictedHomeGoals = p.PredictedHomeGoals,
                    PredictedAwayGoals = p.PredictedAwayGoals,
                    MatchId = p.MatchId,
                    HomeTeam = p.Match.HomeTeam,
                    AwayTeam = p.Match.AwayTeam,
                    MatchDate = p.Match.MatchDate
                })
                .ToListAsync();
        }
    }
}