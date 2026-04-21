using Microsoft.EntityFrameworkCore;
using RedZone.Data;
using RedZone.Data.Models.Entities;
using RedZone.Services.Core.Interfaces;
using RedZone.ViewModels.Comment;

namespace RedZone.Services.Core
{
    public class CommentService : ICommentService
    {
        private readonly RedZoneDbContext context;

        public CommentService(RedZoneDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<CommentViewModel>> GetMatchCommentsAsync(
            int matchId,
            string? currentUserId,
            bool isAdmin)
        {
            return await this.context.Comments
                .Where(c => c.MatchId == matchId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserId = c.UserId,
                    UserName = c.User.UserName ?? "Unknown",
                    CanDelete = isAdmin || c.UserId == currentUserId
                })
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(CommentCreateViewModel model, string userId)
        {
            if (string.IsNullOrWhiteSpace(model.Content))
            {
                return false;
            }

            bool matchExists = await this.context.Matches
                .AnyAsync(m => m.Id == model.MatchId);

            if (!matchExists)
            {
                return false;
            }

            var comment = new Comment
            {
                MatchId = model.MatchId,
                Content = model.Content.Trim(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await this.context.Comments.AddAsync(comment);
            await this.context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int commentId, string userId, bool isAdmin)
        {
            var comment = await this.context.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                return false;
            }

            if (!isAdmin && comment.UserId != userId)
            {
                return false;
            }

            this.context.Comments.Remove(comment);
            await this.context.SaveChangesAsync();

            return true;
        }

        public async Task<int> GetLastCommentIdAsync(int matchId, string userId)
        {
            return await this.context.Comments
                .Where(c => c.MatchId == matchId && c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();
        }
    }
}