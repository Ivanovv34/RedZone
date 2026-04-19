using Microsoft.EntityFrameworkCore;
using RedZone.Data;
using RedZone.Data.Models.Entities;
using RedZone.Data.Models.Enums;
using RedZone.Services.Core.Interfaces;
using RedZone.ViewModels.Competition;
using RedZone.ViewModels.Match;

namespace RedZone.Services.Core
{
    public class MatchService : IMatchService
    {
        private readonly RedZoneDbContext context;

        public MatchService(RedZoneDbContext context)
        {
            this.context = context;
        }

        public async Task<MatchIndexPageViewModel> GetAllAsync(
            string? userId = null,
            int page = 1,
            int pageSize = 10)
        {
            if (page < 1)
            {
                page = 1;
            }

            var baseQuery = this.context.Matches
                .Include(m => m.Competition)
                .OrderBy(m => m.MatchDate)
                .AsQueryable();

            int totalCount = await baseQuery.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (totalPages == 0)
            {
                totalPages = 1;
            }

            if (page > totalPages)
            {
                page = totalPages;
            }

            var matches = await baseQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MatchIndexViewModel
                {
                    Id = m.Id,
                    HomeTeam = m.HomeTeam,
                    AwayTeam = m.AwayTeam,
                    MatchDate = m.MatchDate,
                    CompetitionName = m.Competition.Name,
                    Status = m.Status
                })
                .ToListAsync();

            if (userId != null && matches.Any())
            {
                var pageMatchIds = matches
                    .Select(m => m.Id)
                    .ToList();

                var predictedMatchIds = await this.context.Predictions
                    .Where(p => p.UserId == userId && pageMatchIds.Contains(p.MatchId))
                    .Select(p => p.MatchId)
                    .ToListAsync();

                foreach (var match in matches)
                {
                    match.HasPredicted = predictedMatchIds.Contains(match.Id);
                }
            }

            return new MatchIndexPageViewModel
            {
                Matches = matches,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<MatchDetailsViewModel?> GetByIdAsync(int id)
        {
            return await this.context.Matches
                .Include(m => m.Competition)
                .Where(m => m.Id == id)
                .Select(m => new MatchDetailsViewModel
                {
                    Id = m.Id,
                    HomeTeam = m.HomeTeam,
                    AwayTeam = m.AwayTeam,
                    MatchDate = m.MatchDate,
                    CompetitionName = m.Competition.Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(MatchCreateViewModel model)
        {
            var match = new Match
            {
                HomeTeam = model.HomeTeam,
                AwayTeam = model.AwayTeam,
                MatchDate = model.MatchDate,
                CompetitionId = model.CompetitionId
            };

            await this.context.Matches.AddAsync(match);
            await this.context.SaveChangesAsync();
        }

        public async Task<MatchEditViewModel?> GetForEditAsync(int id)
        {
            return await this.context.Matches
                .Where(m => m.Id == id)
                .Select(m => new MatchEditViewModel
                {
                    Id = m.Id,
                    HomeTeam = m.HomeTeam,
                    AwayTeam = m.AwayTeam,
                    MatchDate = m.MatchDate,
                    CompetitionId = m.CompetitionId
                })
                .FirstOrDefaultAsync();
        }

        public async Task EditAsync(int id, MatchEditViewModel model)
        {
            var match = await this.context.Matches.FindAsync(id);

            if (match != null)
            {
                match.HomeTeam = model.HomeTeam;
                match.AwayTeam = model.AwayTeam;
                match.MatchDate = model.MatchDate;
                match.CompetitionId = model.CompetitionId;

                await this.context.SaveChangesAsync();
            }
        }

        public async Task<MatchDeleteViewModel?> GetForDeleteAsync(int id)
        {
            return await this.context.Matches
                .Include(m => m.Competition)
                .Where(m => m.Id == id)
                .Select(m => new MatchDeleteViewModel
                {
                    Id = m.Id,
                    HomeTeam = m.HomeTeam,
                    AwayTeam = m.AwayTeam,
                    MatchDate = m.MatchDate,
                    CompetitionName = m.Competition.Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var match = await this.context.Matches.FindAsync(id);

            if (match != null)
            {
                this.context.Matches.Remove(match);
                await this.context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CompetitionViewModel>> GetAllCompetitionsAsync()
        {
            return await this.context.Competitions
                .Select(c => new CompetitionViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task EnterResultAsync(int matchId, EnterMatchResultViewModel model)
        {
            var match = await this.context.Matches
                .Include(m => m.Result)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
            {
                throw new ArgumentException("Match not found.");
            }

            if (match.Result == null)
            {
                match.Result = new MatchResult
                {
                    MatchId = matchId,
                    HomeGoals = model.HomeGoals,
                    AwayGoals = model.AwayGoals,
                    EnteredAt = DateTime.UtcNow
                };
            }
            else
            {
                match.Result.HomeGoals = model.HomeGoals;
                match.Result.AwayGoals = model.AwayGoals;
                match.Result.EnteredAt = DateTime.UtcNow;
            }

            match.Status = MatchStatus.Finished;

            await this.context.SaveChangesAsync();
        }
    }
}