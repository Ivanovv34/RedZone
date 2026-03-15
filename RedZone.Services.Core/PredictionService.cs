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

        public async Task<PredictionCreateViewModel?> GetPredictionFormAsync(int matchId)
        {
            return await context.Matches
                .Where(m => m.Id == matchId)
                .Select(m => new PredictionCreateViewModel
                {
                    MatchId = m.Id,
                    HomeTeam = m.HomeTeam,
                    AwayTeam = m.AwayTeam
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(PredictionCreateViewModel model, string userId)
        {
            var prediction = new Prediction
            {
                MatchId = model.MatchId,
                PredictedHomeGoals = model.PredictedHomeGoals,
                PredictedAwayGoals = model.PredictedAwayGoals,
                UserId = userId
            };

            await context.Predictions.AddAsync(prediction);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PredictionViewModel>> GetUserPredictionsAsync(string userId)
        {
            return await context.Predictions
                .Where(p => p.UserId == userId)
                .Include(p => p.Match)
                .Select(p => new PredictionViewModel
                {
                    Id = p.Id,
                    HomeTeam = p.Match.HomeTeam,
                    AwayTeam = p.Match.AwayTeam,
                    MatchDate = p.Match.MatchDate,
                    PredictedHomeGoals = p.PredictedHomeGoals,
                    PredictedAwayGoals = p.PredictedAwayGoals
                })
                .ToListAsync();
        }
    }
}