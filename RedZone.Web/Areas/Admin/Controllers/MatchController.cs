using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedZone.Services.Core.Interfaces;
using RedZone.ViewModels.Match;

namespace RedZone.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MatchController : Controller
    {
        private readonly IMatchService matchService;
        private readonly IPredictionService predictionService;

        public MatchController(
            IMatchService matchService,
            IPredictionService predictionService)
        {
            this.matchService = matchService;
            this.predictionService = predictionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await this.matchService.GetAllAsync();
            return this.View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var competitions = await this.matchService.GetAllCompetitionsAsync();

            var model = new MatchCreateViewModel
            {
                Competitions = competitions
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MatchCreateViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                var competitions = await this.matchService.GetAllCompetitionsAsync();
                model.Competitions = competitions;
                return this.View(model);
            }

            await this.matchService.CreateAsync(model);

            this.TempData["Toast"] = "Match created successfully.";
            this.TempData["ToastType"] = "success";

            return this.RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await this.matchService.GetForEditAsync(id);

            if (model == null)
            {
                return this.NotFound();
            }

            var competitions = await this.matchService.GetAllCompetitionsAsync();
            model.Competitions = competitions;

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MatchEditViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                var competitions = await this.matchService.GetAllCompetitionsAsync();
                model.Competitions = competitions;
                return this.View(model);
            }

            var existingMatch = await this.matchService.GetForEditAsync(id);

            if (existingMatch == null)
            {
                return this.NotFound();
            }

            await this.matchService.EditAsync(id, model);

            this.TempData["Toast"] = "Match updated successfully.";
            this.TempData["ToastType"] = "success";

            return this.RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await this.matchService.GetForDeleteAsync(id);

            if (model == null)
            {
                return this.NotFound();
            }

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var existingMatch = await this.matchService.GetForDeleteAsync(id);

            if (existingMatch == null)
            {
                return this.NotFound();
            }

            await this.matchService.DeleteAsync(id);

            this.TempData["Toast"] = "Match deleted successfully.";
            this.TempData["ToastType"] = "success";

            return this.RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> EnterResult(int id)
        {
            var match = await this.matchService.GetByIdAsync(id);

            if (match == null)
            {
                return this.NotFound();
            }

            var model = new EnterMatchResultViewModel
            {
                MatchId = match.Id,
                HomeTeam = match.HomeTeam,
                AwayTeam = match.AwayTeam
            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnterResult(int id, EnterMatchResultViewModel model)
        {
            if (id != model.MatchId)
            {
                return this.BadRequest();
            }

            var match = await this.matchService.GetByIdAsync(id);

            if (match == null)
            {
                return this.NotFound();
            }

            if (!this.ModelState.IsValid)
            {
                model.HomeTeam = match.HomeTeam;
                model.AwayTeam = match.AwayTeam;
                return this.View(model);
            }

            await this.matchService.EnterResultAsync(id, model);
            await this.predictionService.CalculatePointsAsync(id);

            this.TempData["Toast"] = "Match result saved. Points calculated!";
            this.TempData["ToastType"] = "success";

            return this.RedirectToAction(nameof(Index));
        }
    }
}