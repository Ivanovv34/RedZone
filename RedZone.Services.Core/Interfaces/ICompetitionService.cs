using RedZone.ViewModels.Competition;

namespace RedZone.Services.Core.Interfaces
{
    public interface ICompetitionService
    {
        Task<IEnumerable<CompetitionViewModel>> GetAllAsync();

        Task<CompetitionViewModel?> GetByIdAsync(int id);

        Task CreateAsync(CompetitionViewModel model);

        Task EditAsync(int id, CompetitionViewModel model);

        Task DeleteAsync(int id);
    }
}