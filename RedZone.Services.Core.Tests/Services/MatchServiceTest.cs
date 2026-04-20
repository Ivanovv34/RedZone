using Microsoft.EntityFrameworkCore;
using RedZone.Data.Models.Entities;
using RedZone.Data.Models.Enums;
using RedZone.Services.Core;
using RedZone.Services.Core.Tests.Helpers;
using RedZone.ViewModels.Match;

namespace RedZone.Services.Core.Tests.Services
{
    public class MatchServiceTests
    {
        [Fact]
        public async Task EnterResultAsync_ShouldCreateResult_WhenMatchHasNoResult()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            context.Competitions.Add(new Competition
            {
                Id = 1,
                Name = "Premier League"
            });

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

            var service = new MatchService(context);

            var model = new EnterMatchResultViewModel
            {
                MatchId = 1,
                HomeGoals = 2,
                AwayGoals = 1
            };

            await service.EnterResultAsync(1, model);

            var match = await context.Matches
                .Include(m => m.Result)
                .FirstAsync(m => m.Id == 1);

            Assert.NotNull(match.Result);
            Assert.Equal(2, match.Result!.HomeGoals);
            Assert.Equal(1, match.Result.AwayGoals);
        }

        [Fact]
        public async Task EnterResultAsync_ShouldUpdateResult_WhenMatchAlreadyHasResult()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            context.Competitions.Add(new Competition
            {
                Id = 1,
                Name = "Premier League"
            });

            context.Matches.Add(new Match
            {
                Id = 1,
                HomeTeam = "Liverpool",
                AwayTeam = "Chelsea",
                MatchDate = DateTime.UtcNow.AddDays(1),
                CompetitionId = 1,
                Status = MatchStatus.Upcoming,
                Result = new MatchResult
                {
                    MatchId = 1,
                    HomeGoals = 1,
                    AwayGoals = 1,
                    EnteredAt = DateTime.UtcNow.AddMinutes(-10)
                }
            });

            await context.SaveChangesAsync();

            var service = new MatchService(context);

            var model = new EnterMatchResultViewModel
            {
                MatchId = 1,
                HomeGoals = 3,
                AwayGoals = 0
            };

            await service.EnterResultAsync(1, model);

            var match = await context.Matches
                .Include(m => m.Result)
                .FirstAsync(m => m.Id == 1);

            Assert.NotNull(match.Result);
            Assert.Equal(3, match.Result!.HomeGoals);
            Assert.Equal(0, match.Result.AwayGoals);
        }

        [Fact]
        public async Task EnterResultAsync_ShouldSetMatchStatusToFinished()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            context.Competitions.Add(new Competition
            {
                Id = 1,
                Name = "Champions League"
            });

            context.Matches.Add(new Match
            {
                Id = 1,
                HomeTeam = "Liverpool",
                AwayTeam = "Real Madrid",
                MatchDate = DateTime.UtcNow.AddDays(1),
                CompetitionId = 1,
                Status = MatchStatus.Upcoming
            });

            await context.SaveChangesAsync();

            var service = new MatchService(context);

            var model = new EnterMatchResultViewModel
            {
                MatchId = 1,
                HomeGoals = 2,
                AwayGoals = 0
            };

            await service.EnterResultAsync(1, model);

            var match = await context.Matches.FirstAsync(m => m.Id == 1);

            Assert.Equal(MatchStatus.Finished, match.Status);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedMatches()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            context.Competitions.Add(new Competition
            {
                Id = 1,
                Name = "Premier League"
            });

            for (int i = 1; i <= 12; i++)
            {
                context.Matches.Add(new Match
                {
                    Id = i,
                    HomeTeam = $"Home {i}",
                    AwayTeam = $"Away {i}",
                    MatchDate = DateTime.UtcNow.AddDays(i),
                    CompetitionId = 1,
                    Status = MatchStatus.Upcoming
                });
            }

            await context.SaveChangesAsync();

            var service = new MatchService(context);

            var result = await service.GetAllAsync(null, 2, 5);

            Assert.Equal(2, result.CurrentPage);
            Assert.Equal(3, result.TotalPages);
            Assert.Equal(12, result.TotalCount);
            Assert.Equal(5, result.Matches.Count());
        }

        [Fact]
        public async Task GetAllAsync_ShouldMarkHasPredicted_ForUserPredictions()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            context.Competitions.Add(new Competition
            {
                Id = 1,
                Name = "Premier League"
            });

            context.Matches.AddRange(
                new Match
                {
                    Id = 1,
                    HomeTeam = "Liverpool",
                    AwayTeam = "Arsenal",
                    MatchDate = DateTime.UtcNow.AddDays(1),
                    CompetitionId = 1,
                    Status = MatchStatus.Upcoming
                },
                new Match
                {
                    Id = 2,
                    HomeTeam = "Chelsea",
                    AwayTeam = "Liverpool",
                    MatchDate = DateTime.UtcNow.AddDays(2),
                    CompetitionId = 1,
                    Status = MatchStatus.Upcoming
                });

            context.Predictions.Add(new Prediction
            {
                MatchId = 1,
                UserId = "user-1",
                PredictedHomeGoals = 2,
                PredictedAwayGoals = 1
            });

            await context.SaveChangesAsync();

            var service = new MatchService(context);

            var result = await service.GetAllAsync("user-1", 1, 10);
            var matches = result.Matches.ToList();

            Assert.True(matches.Single(m => m.Id == 1).HasPredicted);
            Assert.False(matches.Single(m => m.Id == 2).HasPredicted);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenMatchDoesNotExist()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();
            var service = new MatchService(context);

            var result = await service.GetByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task EnterResultAsync_ShouldThrow_WhenMatchDoesNotExist()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();
            var service = new MatchService(context);

            var model = new EnterMatchResultViewModel
            {
                MatchId = 999,
                HomeGoals = 1,
                AwayGoals = 0
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.EnterResultAsync(999, model));
        }
    }
}