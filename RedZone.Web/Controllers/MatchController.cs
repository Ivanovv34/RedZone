using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedZone.Services.Core.Interfaces;

namespace RedZone.Web.Controllers
{
    public class MatchController : Controller
    {
        private readonly IMatchService matchService;

        public MatchController(IMatchService matchService)
        {
            this.matchService = matchService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await this.matchService.GetAllAsync(userId);

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

            return this.View(model);
        }
    }
}