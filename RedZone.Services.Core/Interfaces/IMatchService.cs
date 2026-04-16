using RedZone.ViewModels.Match;
using RedZone.ViewModels.Competition;

namespace RedZone.Services.Core.Interfaces
{
    public interface IMatchService
    {
        Task<IEnumerable<MatchIndexViewModel>> GetAllAsync(string? userId = null);

        Task EnterResultAsync(int matchId, EnterMatchResultViewModel model);

        Task<MatchDetailsViewModel?> GetByIdAsync(int id);

        Task CreateAsync(MatchCreateViewModel model);

        Task<MatchEditViewModel?> GetForEditAsync(int id);

        Task EditAsync(int id, MatchEditViewModel model);

        Task<MatchDeleteViewModel?> GetForDeleteAsync(int id);

        Task DeleteAsync(int id);

        Task<IEnumerable<CompetitionViewModel>> GetAllCompetitionsAsync();
    }
}