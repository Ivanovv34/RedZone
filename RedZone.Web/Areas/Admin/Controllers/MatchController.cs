using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RedZone.Services.Core.Interfaces;
using RedZone.ViewModels.Match;
using RedZone.Web.Hubs;

namespace RedZone.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MatchController : Controller
    {
        private readonly IMatchService matchService;
        private readonly IPredictionService predictionService;
        private readonly IHubContext<LeaderboardHub> hubContext;

        public MatchController(
            IMatchService matchService,
            IPredictionService predictionService,
            IHubContext<LeaderboardHub> hubContext)
        {
            this.matchService = matchService;
            this.predictionService = predictionService;
            this.hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await matchService.GetAllAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var competitions = await matchService.GetAllCompetitionsAsync();
            var model = new MatchCreateViewModel { Competitions = competitions };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MatchCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var competitions = await matchService.GetAllCompetitionsAsync();
                model.Competitions = competitions;
                return View(model);
            }

            await matchService.CreateAsync(model);
            TempData["Toast"] = "Match created successfully.";
            TempData["ToastType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await matchService.GetForEditAsync(id);
            if (model == null) return NotFound();

            var competitions = await matchService.GetAllCompetitionsAsync();
            model.Competitions = competitions;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MatchEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var competitions = await matchService.GetAllCompetitionsAsync();
                model.Competitions = competitions;
                return View(model);
            }

            var existingMatch = await matchService.GetForEditAsync(id);
            if (existingMatch == null) return NotFound();

            await matchService.EditAsync(id, model);
            TempData["Toast"] = "Match updated successfully.";
            TempData["ToastType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await matchService.GetForDeleteAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var existingMatch = await matchService.GetForDeleteAsync(id);
            if (existingMatch == null) return NotFound();

            await matchService.DeleteAsync(id);
            TempData["Toast"] = "Match deleted successfully.";
            TempData["ToastType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> EnterResult(int id)
        {
            var match = await matchService.GetByIdAsync(id);
            if (match == null) return NotFound();

            var model = new EnterMatchResultViewModel
            {
                MatchId = match.Id,
                HomeTeam = match.HomeTeam,
                AwayTeam = match.AwayTeam
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnterResult(int id, EnterMatchResultViewModel model)
        {
            if (id != model.MatchId) return BadRequest();

            var match = await matchService.GetByIdAsync(id);
            if (match == null) return NotFound();

            if (!ModelState.IsValid)
            {
                model.HomeTeam = match.HomeTeam;
                model.AwayTeam = match.AwayTeam;
                return View(model);
            }

            await matchService.EnterResultAsync(id, model);
            await predictionService.CalculatePointsAsync(id);


            var leaderboard = await predictionService.GetLeaderboardAsync();
            await hubContext.Clients.All.SendAsync("LeaderboardUpdated", leaderboard);

            TempData["Toast"] = "Result saved and points calculated!";
            TempData["ToastType"] = "success";
            return RedirectToAction(nameof(Index));
        }
    }
}