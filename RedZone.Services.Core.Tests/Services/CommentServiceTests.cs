using Microsoft.EntityFrameworkCore;
using RedZone.Data.Models.Entities;
using RedZone.Services.Core;
using RedZone.Services.Core.Tests.Helpers;
using RedZone.ViewModels.Comment;

namespace RedZone.Services.Core.Tests.Services
{
    public class CommentServiceTests
    {
        [Fact]
        public async Task CreateAsync_ShouldReturnFalse_WhenContentIsEmpty()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            context.Matches.Add(new Match
            {
                Id = 1,
                HomeTeam = "Liverpool",
                AwayTeam = "Arsenal",
                CompetitionId = 1,
                MatchDate = DateTime.UtcNow.AddDays(1)
            });

            await context.SaveChangesAsync();

            var service = new CommentService(context);

            var model = new CommentCreateViewModel
            {
                MatchId = 1,
                Content = "   "
            };

            var result = await service.CreateAsync(model, "user-1");

            Assert.False(result);
            Assert.Empty(context.Comments);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnFalse_WhenMatchDoesNotExist()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();
            var service = new CommentService(context);

            var model = new CommentCreateViewModel
            {
                MatchId = 999,
                Content = "Great match!"
            };

            var result = await service.CreateAsync(model, "user-1");

            Assert.False(result);
            Assert.Empty(context.Comments);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateComment_WhenDataIsValid()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            context.Matches.Add(new Match
            {
                Id = 1,
                HomeTeam = "Liverpool",
                AwayTeam = "Chelsea",
                CompetitionId = 1,
                MatchDate = DateTime.UtcNow.AddDays(1)
            });

            await context.SaveChangesAsync();

            var service = new CommentService(context);

            var model = new CommentCreateViewModel
            {
                MatchId = 1,
                Content = "This will be a tough game."
            };

            var result = await service.CreateAsync(model, "user-1");

            var comment = await context.Comments.FirstOrDefaultAsync();

            Assert.True(result);
            Assert.NotNull(comment);
            Assert.Equal(1, comment!.MatchId);
            Assert.Equal("user-1", comment.UserId);
            Assert.Equal("This will be a tough game.", comment.Content);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteComment_WhenUserIsOwner()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            context.Comments.Add(new Comment
            {
                Id = 1,
                MatchId = 1,
                UserId = "user-1",
                Content = "Owner comment"
            });

            await context.SaveChangesAsync();

            var service = new CommentService(context);

            var result = await service.DeleteAsync(1, "user-1", false);

            Assert.True(result);
            Assert.Empty(context.Comments);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteComment_WhenUserIsAdmin()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            context.Comments.Add(new Comment
            {
                Id = 1,
                MatchId = 1,
                UserId = "user-1",
                Content = "Some comment"
            });

            await context.SaveChangesAsync();

            var service = new CommentService(context);

            var result = await service.DeleteAsync(1, "admin-user", true);

            Assert.True(result);
            Assert.Empty(context.Comments);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenUserIsNotOwnerAndNotAdmin()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            context.Comments.Add(new Comment
            {
                Id = 1,
                MatchId = 1,
                UserId = "user-1",
                Content = "Protected comment"
            });

            await context.SaveChangesAsync();

            var service = new CommentService(context);

            var result = await service.DeleteAsync(1, "user-2", false);

            Assert.False(result);
            Assert.Single(context.Comments);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenCommentDoesNotExist()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();
            var service = new CommentService(context);

            var result = await service.DeleteAsync(999, "user-1", false);

            Assert.False(result);
        }
    }
}