using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedZone.Services.Core.Interfaces;

namespace RedZone.Web.Controllers
{
    public class MatchController : Controller
    {
        private readonly IMatchService matchService;
        private readonly ICommentService commentService;

        public MatchController(
            IMatchService matchService,
            ICommentService commentService)
        {
            this.matchService = matchService;
            this.commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await this.matchService.GetAllAsync(userId, page, 10);

            return this.View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var model = await this.matchService.GetByIdAsync(id);

            if (model == null)
            {
                return this.NotFound();
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isAdmin = this.User.IsInRole("Admin");

            model.Comments = await this.commentService.GetMatchCommentsAsync(id, userId, isAdmin);
            model.NewComment.MatchId = id;

            return this.View(model);
        }
    }
}