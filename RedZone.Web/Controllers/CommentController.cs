using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedZone.Services.Core.Interfaces;
using RedZone.ViewModels.Comment;

namespace RedZone.Web.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ICommentService commentService;

        public CommentController(ICommentService commentService)
        {
            this.commentService = commentService;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommentCreateViewModel model)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!this.ModelState.IsValid)
            {
                this.TempData["Toast"] = "Comment must be between 2 and 500 characters.";
                this.TempData["ToastType"] = "danger";
                return this.RedirectToAction("Details", "Match", new { id = model.MatchId });
            }

            var success = await this.commentService.CreateAsync(model, userId);

            if (success)
            {
                this.TempData["Toast"] = "Comment added successfully.";
                this.TempData["ToastType"] = "success";
            }
            else
            {
                this.TempData["Toast"] = "Unable to add comment.";
                this.TempData["ToastType"] = "danger";
            }

            return this.RedirectToAction("Details", "Match", new { id = model.MatchId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int matchId)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isAdmin = this.User.IsInRole("Admin");

            var success = await this.commentService.DeleteAsync(id, userId, isAdmin);

            if (success)
            {
                this.TempData["Toast"] = "Comment deleted.";
                this.TempData["ToastType"] = "info";
            }
            else
            {
                this.TempData["Toast"] = "You cannot delete this comment.";
                this.TempData["ToastType"] = "danger";
            }

            return this.RedirectToAction("Details", "Match", new { id = matchId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAjax([FromBody] CommentCreateViewModel model)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = this.User.Identity?.Name ?? "Unknown";
            bool isAdmin = this.User.IsInRole("Admin");

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(new { error = "Comment must be between 2 and 500 characters." });
            }

            var success = await this.commentService.CreateAsync(model, userId);

            if (!success)
            {
                return this.BadRequest(new { error = "Unable to add comment." });
            }

            // Return the new comment so JS can append it immediately
            var newComment = new CommentViewModel
            {
                Id = await this.commentService.GetLastCommentIdAsync(model.MatchId, userId),
                Content = model.Content.Trim(),
                CreatedAt = DateTime.UtcNow,
                UserId = userId!,
                UserName = userName,
                CanDelete = true
            };

            return this.Json(newComment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAjax([FromBody] DeleteCommentRequest request)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isAdmin = this.User.IsInRole("Admin");

            var success = await this.commentService.DeleteAsync(request.CommentId, userId, isAdmin);

            if (!success)
            {
                return this.BadRequest(new { error = "You cannot delete this comment." });
            }

            return this.Json(new { success = true });
        }
    }

    public class DeleteCommentRequest
    {
        public int CommentId { get; set; }
    }
}