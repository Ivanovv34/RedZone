using RedZone.ViewModels.Competition;

namespace RedZone.Services.Core.Interfaces
{
    public interface ICompetitionService
    {
        Task<IEnumerable<CompetitionViewModel>> GetAllAsync();
    }
}