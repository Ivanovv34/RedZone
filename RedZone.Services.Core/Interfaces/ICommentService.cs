using RedZone.ViewModels.Comment;

namespace RedZone.Services.Core.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentViewModel>> GetMatchCommentsAsync(
            int matchId,
            string? currentUserId,
            bool isAdmin);

        Task<bool> CreateAsync(CommentCreateViewModel model, string userId);

        Task<bool> DeleteAsync(int commentId, string userId, bool isAdmin);

        Task<int> GetLastCommentIdAsync(int matchId, string userId);
    }
}