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
    }
}