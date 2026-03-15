using Microsoft.EntityFrameworkCore;
using RedZone.Data;
using RedZone.Data.Models.Entities;
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

        public async Task<IEnumerable<MatchIndexViewModel>> GetAllAsync()
        {
            return await context.Matches
                .Include(m => m.Competition)
                .OrderBy(m => m.MatchDate)
                .Select(m => new MatchIndexViewModel
                {
                    Id = m.Id,
                    HomeTeam = m.HomeTeam,
                    AwayTeam = m.AwayTeam,
                    MatchDate = m.MatchDate,
                    CompetitionName = m.Competition.Name
                })
                .ToListAsync();
        }

        public async Task<MatchDetailsViewModel?> GetByIdAsync(int id)
        {
            return await context.Matches
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

            await context.Matches.AddAsync(match);
            await context.SaveChangesAsync();
        }

        public async Task<MatchEditViewModel?> GetForEditAsync(int id)
        {
            return await context.Matches
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
            var match = await context.Matches.FindAsync(id);

            if (match != null)
            {
                match.HomeTeam = model.HomeTeam;
                match.AwayTeam = model.AwayTeam;
                match.MatchDate = model.MatchDate;
                match.CompetitionId = model.CompetitionId;

                await context.SaveChangesAsync();
            }
        }

        public async Task<MatchDeleteViewModel?> GetForDeleteAsync(int id)
        {
            return await context.Matches
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
            var match = await context.Matches.FindAsync(id);

            if (match != null)
            {
                context.Matches.Remove(match);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CompetitionViewModel>> GetAllCompetitionsAsync()
        {
            return await context.Competitions
                .Select(c => new CompetitionViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }
    }
}