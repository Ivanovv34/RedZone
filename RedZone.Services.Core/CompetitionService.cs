using Microsoft.EntityFrameworkCore;
using RedZone.Data;
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
    }
}