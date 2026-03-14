using Microsoft.EntityFrameworkCore;
using RedZone.Data;
using RedZone.Data.Models.Entities;
using RedZone.Services.Core.Interfaces;
using RedZone.ViewModels.Competition;

namespace RedZone.Services.Core
{
    public class CompetitionService : ICompetitionService
    {
        private readonly RedZoneDbContext context;

        public CompetitionService(RedZoneDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<CompetitionViewModel>> GetAllAsync()
        {
            return await context.Competitions
                .Select(c => new CompetitionViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<CompetitionViewModel?> GetByIdAsync(int id)
        {
            return await context.Competitions
                .Where(c => c.Id == id)
                .Select(c => new CompetitionViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(CompetitionViewModel model)
        {
            var competition = new Competition
            {
                Name = model.Name
            };

            await context.Competitions.AddAsync(competition);
            await context.SaveChangesAsync();
        }

        public async Task EditAsync(int id, CompetitionViewModel model)
        {
            var competition = await context.Competitions.FindAsync(id);

            if (competition != null)
            {
                competition.Name = model.Name;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var competition = await context.Competitions.FindAsync(id);

            if (competition != null)
            {
                context.Competitions.Remove(competition);
                await context.SaveChangesAsync();
            }
        }
    }
}