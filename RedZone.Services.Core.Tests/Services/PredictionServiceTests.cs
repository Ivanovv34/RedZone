using Microsoft.EntityFrameworkCore;
using RedZone.Data;
using RedZone.Data.Models.Entities;
using RedZone.Data.Models.Enums;
using RedZone.Services.Core;
using RedZone.Services.Core.Tests.Helpers;
using RedZone.ViewModels.Prediction;

namespace RedZone.Services.Core.Tests.Services
{
    public class PredictionServiceTests
    {
        [Fact]
        public async Task CreateAsync_ShouldCreatePrediction_WhenMatchExistsAndIsUpcoming()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            context.Matches.Add(new Match
            {
                Id = 1,
                HomeTeam = "Liverpool",
                AwayTeam = "Arsenal",
                MatchDate = DateTime.UtcNow.AddDays(1),
                CompetitionId = 1,
                Status = MatchStatus.Upcoming
            });

            await context.SaveChangesAsync();

            var service = new PredictionService(context);

            var model = new PredictionCreateViewModel
            {
                MatchId = 1,
                PredictedHomeGoals = 2,
                PredictedAwayGoals = 1
            };

            await service.CreateAsync(model, "user-1");

            var prediction = await context.Predictions.FirstOrDefaultAsync();

            Assert.NotNull(prediction);
            Assert.Equal(1, prediction!.MatchId);
            Assert.Equal("user-1", prediction.UserId);
            Assert.Equal(2, prediction.PredictedHomeGoals);
            Assert.Equal(1, prediction.PredictedAwayGoals);
        }

        [Fact]
        public async Task CreateAsync_ShouldNotCreatePrediction_WhenMatchDoesNotExist()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();
            var service = new PredictionService(context);

            var model = new PredictionCreateViewModel
            {
                MatchId = 999,
                PredictedHomeGoals = 1,
                PredictedAwayGoals = 0
            };

            await service.CreateAsync(model, "user-1");

            Assert.Empty(context.Predictions);
        }

        [Fact]
        public async Task CreateAsync_ShouldNotCreatePrediction_WhenMatchIsFinished()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            context.Matches.Add(new Match
            {
                Id = 1,
                HomeTeam = "Liverpool",
                AwayTeam = "Chelsea",
                MatchDate = DateTime.UtcNow.AddDays(-1),
                CompetitionId = 1,
                Status = MatchStatus.Finished
            });

            await context.SaveChangesAsync();

            var service = new PredictionService(context);

            var model = new PredictionCreateViewModel
            {
                MatchId = 1,
                PredictedHomeGoals = 3,
                PredictedAwayGoals = 0
            };

            await service.CreateAsync(model, "user-1");

            Assert.Empty(context.Predictions);
        }

        [Fact]
        public async Task CreateAsync_ShouldNotCreateDuplicatePrediction_ForSameUserAndMatch()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            context.Matches.Add(new Match
            {
                Id = 1,
                HomeTeam = "Liverpool",
                AwayTeam = "PSG",
                MatchDate = DateTime.UtcNow.AddDays(1),
                CompetitionId = 1,
                Status = MatchStatus.Upcoming
            });

            context.Predictions.Add(new Prediction
            {
                MatchId = 1,
                UserId = "user-1",
                PredictedHomeGoals = 1,
                PredictedAwayGoals = 1
            });

            await context.SaveChangesAsync();

            var service = new PredictionService(context);

            var model = new PredictionCreateViewModel
            {
                MatchId = 1,
                PredictedHomeGoals = 2,
                PredictedAwayGoals = 0
            };

            await service.CreateAsync(model, "user-1");

            Assert.Single(context.Predictions);
        }

        [Fact]
        public async Task CalculatePointsAsync_ShouldGive3Points_ForExactScore()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            var match = new Match
            {
                Id = 1,
                HomeTeam = "Liverpool",
                AwayTeam = "Real Madrid",
                MatchDate = DateTime.UtcNow,
                CompetitionId = 1,
                Status = MatchStatus.Finished,
                Result = new MatchResult
                {
                    MatchId = 1,
                    HomeGoals = 2,
                    AwayGoals = 1,
                    EnteredAt = DateTime.UtcNow
                },
                Predictions = new List<Prediction>
                {
                    new Prediction
                    {
                        UserId = "user-1",
                        PredictedHomeGoals = 2,
                        PredictedAwayGoals = 1
                    }
                }
            };

            context.Matches.Add(match);
            await context.SaveChangesAsync();

            var service = new PredictionService(context);

            await service.CalculatePointsAsync(1);

            var prediction = await context.Predictions.FirstAsync();

            Assert.True(prediction.IsCalculated);
            Assert.Equal(3, prediction.PointsEarned);
        }

        [Fact]
        public async Task CalculatePointsAsync_ShouldGive1Point_ForCorrectOutcome()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            var match = new Match
            {
                Id = 1,
                HomeTeam = "Liverpool",
                AwayTeam = "Tottenham",
                MatchDate = DateTime.UtcNow,
                CompetitionId = 1,
                Status = MatchStatus.Finished,
                Result = new MatchResult
                {
                    MatchId = 1,
                    HomeGoals = 2,
                    AwayGoals = 0,
                    EnteredAt = DateTime.UtcNow
                },
                Predictions = new List<Prediction>
                {
                    new Prediction
                    {
                        UserId = "user-1",
                        PredictedHomeGoals = 3,
                        PredictedAwayGoals = 1
                    }
                }
            };

            context.Matches.Add(match);
            await context.SaveChangesAsync();

            var service = new PredictionService(context);

            await service.CalculatePointsAsync(1);

            var prediction = await context.Predictions.FirstAsync();

            Assert.True(prediction.IsCalculated);
            Assert.Equal(1, prediction.PointsEarned);
        }

        [Fact]
        public async Task CalculatePointsAsync_ShouldGive0Points_ForWrongPrediction()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            var match = new Match
            {
                Id = 1,
                HomeTeam = "Liverpool",
                AwayTeam = "Manchester City",
                MatchDate = DateTime.UtcNow,
                CompetitionId = 1,
                Status = MatchStatus.Finished,
                Result = new MatchResult
                {
                    MatchId = 1,
                    HomeGoals = 1,
                    AwayGoals = 0,
                    EnteredAt = DateTime.UtcNow
                },
                Predictions = new List<Prediction>
                {
                    new Prediction
                    {
                        UserId = "user-1",
                        PredictedHomeGoals = 0,
                        PredictedAwayGoals = 2
                    }
                }
            };

            context.Matches.Add(match);
            await context.SaveChangesAsync();

            var service = new PredictionService(context);

            await service.CalculatePointsAsync(1);

            var prediction = await context.Predictions.FirstAsync();

            Assert.True(prediction.IsCalculated);
            Assert.Equal(0, prediction.PointsEarned);
        }

        [Fact]
        public async Task GetUserStatsAsync_ShouldReturnCorrectStats()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            context.Predictions.AddRange(
                new Prediction
                {
                    UserId = "user-1",
                    MatchId = 1,
                    PredictedHomeGoals = 2,
                    PredictedAwayGoals = 1,
                    IsCalculated = true,
                    PointsEarned = 3
                },
                new Prediction
                {
                    UserId = "user-1",
                    MatchId = 2,
                    PredictedHomeGoals = 1,
                    PredictedAwayGoals = 1,
                    IsCalculated = true,
                    PointsEarned = 1
                },
                new Prediction
                {
                    UserId = "user-1",
                    MatchId = 3,
                    PredictedHomeGoals = 0,
                    PredictedAwayGoals = 2,
                    IsCalculated = true,
                    PointsEarned = 0
                });

            await context.SaveChangesAsync();

            var service = new PredictionService(context);

            var stats = await service.GetUserStatsAsync("user-1");

            Assert.Equal(3, stats.TotalPredictions);
            Assert.Equal(1, stats.ExactScores);
            Assert.Equal(1, stats.CorrectResults);
            Assert.Equal(4, stats.TotalPoints);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeletePrediction_WhenItBelongsToUser()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            context.Predictions.Add(new Prediction
            {
                Id = 1,
                MatchId = 1,
                UserId = "user-1",
                PredictedHomeGoals = 1,
                PredictedAwayGoals = 0
            });

            await context.SaveChangesAsync();

            var service = new PredictionService(context);

            var result = await service.DeleteAsync(1, "user-1");

            Assert.True(result);
            Assert.Empty(context.Predictions);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenPredictionBelongsToAnotherUser()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            context.Predictions.Add(new Prediction
            {
                Id = 1,
                MatchId = 1,
                UserId = "user-1",
                PredictedHomeGoals = 1,
                PredictedAwayGoals = 0
            });

            await context.SaveChangesAsync();

            var service = new PredictionService(context);

            var result = await service.DeleteAsync(1, "user-2");

            Assert.False(result);
            Assert.Single(context.Predictions);
        }
    }
}