using Microsoft.EntityFrameworkCore;
using RedZone.Data;
using RedZone.Data.Models.Entities;
using RedZone.Data.Models.Enums;
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
            return await this.context.Matches
                .Where(m => m.Id == matchId)
                .Select(m => new PredictionCreateViewModel
                {
                    MatchId = m.Id,
                    HomeTeam = m.HomeTeam,
                    AwayTeam = m.AwayTeam
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateAsync(PredictionCreateViewModel model, string userId)
        {
            var match = await this.context.Matches
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == model.MatchId);

            if (match == null)
            {
                return false;
            }

            if (match.Status == MatchStatus.Finished)
            {
                return false;
            }

            bool alreadyExists = await this.context.Predictions
                .AnyAsync(p => p.MatchId == model.MatchId && p.UserId == userId);

            if (alreadyExists)
            {
                return false;
            }

            var prediction = new Prediction
            {
                MatchId = model.MatchId,
                PredictedHomeGoals = model.PredictedHomeGoals,
                PredictedAwayGoals = model.PredictedAwayGoals,
                UserId = userId,
                IsCalculated = false,
                PointsEarned = null
            };

            await this.context.Predictions.AddAsync(prediction);
            await this.context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<PredictionViewModel>> GetUserPredictionsAsync(string userId)
        {
            return await this.context.Predictions
                .Where(p => p.UserId == userId)
                .Include(p => p.Match)
                    .ThenInclude(m => m.Result)
                .OrderByDescending(p => p.Match.MatchDate)
                .Select(p => new PredictionViewModel
                {
                    Id = p.Id,
                    MatchId = p.MatchId,
                    HomeTeam = p.Match.HomeTeam,
                    AwayTeam = p.Match.AwayTeam,
                    MatchDate = p.Match.MatchDate,
                    PredictedHomeGoals = p.PredictedHomeGoals,
                    PredictedAwayGoals = p.PredictedAwayGoals,
                    PointsEarned = p.PointsEarned,
                    IsCalculated = p.IsCalculated,
                    ActualHomeGoals = p.Match.Result != null ? p.Match.Result.HomeGoals : (int?)null,
                    ActualAwayGoals = p.Match.Result != null ? p.Match.Result.AwayGoals : (int?)null
                })
                .ToListAsync();
        }

        public async Task<PredictionMineViewModel> GetUserPredictionsPagedAsync(
            string userId,
            int page = 1,
            int pageSize = 10)
        {
            if (page < 1) page = 1;

            var baseQuery = this.context.Predictions
                .Where(p => p.UserId == userId)
                .Include(p => p.Match)
                    .ThenInclude(m => m.Result)
                .OrderByDescending(p => p.Match.MatchDate)
                .AsQueryable();

            int totalCount = await baseQuery.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (totalPages == 0) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var predictions = await baseQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PredictionViewModel
                {
                    Id = p.Id,
                    MatchId = p.MatchId,
                    HomeTeam = p.Match.HomeTeam,
                    AwayTeam = p.Match.AwayTeam,
                    MatchDate = p.Match.MatchDate,
                    PredictedHomeGoals = p.PredictedHomeGoals,
                    PredictedAwayGoals = p.PredictedAwayGoals,
                    PointsEarned = p.PointsEarned,
                    IsCalculated = p.IsCalculated,
                    ActualHomeGoals = p.Match.Result != null ? p.Match.Result.HomeGoals : (int?)null,
                    ActualAwayGoals = p.Match.Result != null ? p.Match.Result.AwayGoals : (int?)null
                })
                .ToListAsync();

            return new PredictionMineViewModel
            {
                Predictions = predictions,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<bool> HasUserPredictedAsync(int matchId, string userId)
        {
            return await this.context.Predictions
                .AnyAsync(p => p.MatchId == matchId && p.UserId == userId);
        }

        public async Task<bool> DeleteAsync(int predictionId, string userId)
        {
            var prediction = await this.context.Predictions
                .FirstOrDefaultAsync(p => p.Id == predictionId && p.UserId == userId);

            if (prediction == null) return false;

            this.context.Predictions.Remove(prediction);
            await this.context.SaveChangesAsync();
            return true;
        }

        public async Task CalculatePointsAsync(int matchId)
        {
            var match = await this.context.Matches
                .Include(m => m.Result)
                .Include(m => m.Predictions)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match?.Result == null) return;

            int actualHomeGoals = match.Result.HomeGoals;
            int actualAwayGoals = match.Result.AwayGoals;
            int actualOutcome = GetOutcome(actualHomeGoals, actualAwayGoals);

            foreach (var prediction in match.Predictions)
            {
                int predictedOutcome = GetOutcome(
                    prediction.PredictedHomeGoals,
                    prediction.PredictedAwayGoals);

                if (prediction.PredictedHomeGoals == actualHomeGoals &&
                    prediction.PredictedAwayGoals == actualAwayGoals)
                {
                    prediction.PointsEarned = 3;
                }
                else if (predictedOutcome == actualOutcome)
                {
                    prediction.PointsEarned = 1;
                }
                else
                {
                    prediction.PointsEarned = 0;
                }

                prediction.IsCalculated = true;
            }

            await this.context.SaveChangesAsync();
        }

        public async Task<UserStatsViewModel> GetUserStatsAsync(string userId)
        {
            var predictions = await this.context.Predictions
                .Where(p => p.UserId == userId)
                .ToListAsync();

            return new UserStatsViewModel
            {
                TotalPredictions = predictions.Count,
                ExactScores = predictions.Count(p => p.IsCalculated && p.PointsEarned == 3),
                CorrectResults = predictions.Count(p => p.IsCalculated && p.PointsEarned == 1),
                TotalPoints = predictions
                    .Where(p => p.IsCalculated)
                    .Sum(p => p.PointsEarned ?? 0)
            };
        }

        public async Task<IEnumerable<LeaderboardEntryViewModel>> GetLeaderboardAsync()
        {
            var leaderboard = await this.context.Predictions
                .Where(p => p.IsCalculated)
                .GroupBy(p => new { p.UserId, p.User.UserName })
                .Select(g => new LeaderboardEntryViewModel
                {
                    UserId = g.Key.UserId,
                    UserName = g.Key.UserName ?? "Unknown",
                    TotalPoints = g.Sum(p => p.PointsEarned ?? 0),
                    TotalPredictions = g.Count(),
                    ExactScores = g.Count(p => p.PointsEarned == 3),
                    CorrectResults = g.Count(p => p.PointsEarned == 1)
                })
                .OrderByDescending(x => x.TotalPoints)
                .ThenByDescending(x => x.ExactScores)
                .ThenByDescending(x => x.TotalPredictions)
                .ToListAsync(); 

            for (int i = 0; i < leaderboard.Count; i++)
            {
                leaderboard[i].Position = i + 1;
            }

            return leaderboard;
        }

        private static int GetOutcome(int homeGoals, int awayGoals)
        {
            if (homeGoals > awayGoals) return 1;
            if (awayGoals > homeGoals) return -1;
            return 0;
        }
    }
}