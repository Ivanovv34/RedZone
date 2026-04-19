using RedZone.ViewModels.Competition;
using RedZone.ViewModels.Match;

namespace RedZone.Services.Core.Interfaces
{
    public interface IMatchService
    {
        Task<MatchIndexPageViewModel> GetAllAsync(
            string? userId = null,
            int page = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? competitionTerm = null);

        Task<MatchDetailsViewModel?> GetByIdAsync(int id);

        Task CreateAsync(MatchCreateViewModel model);

        Task<MatchEditViewModel?> GetForEditAsync(int id);

        Task EditAsync(int id, MatchEditViewModel model);

        Task<MatchDeleteViewModel?> GetForDeleteAsync(int id);

        Task DeleteAsync(int id);

        Task<IEnumerable<CompetitionViewModel>> GetAllCompetitionsAsync();

        Task EnterResultAsync(int matchId, EnterMatchResultViewModel model);

        
    }
}