using RedZone.ViewModels;

namespace RedZone.Services.Core.Contracts
{
    public interface ICompetitionService
    {
        Task<IEnumerable<CompetitionViewModel>> GetAllAsync();
    }
}