using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedZone.Services.Core.Interfaces;
using RedZone.ViewModels.Prediction;

namespace RedZone.Web.Controllers
{
    [Authorize]
    public class PredictionController : Controller
    {
        private readonly IPredictionService predictionService;

        public PredictionController(IPredictionService predictionService)
        {
            this.predictionService = predictionService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int matchId)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (await this.predictionService.HasUserPredictedAsync(matchId, userId))
            {
                this.TempData["Toast"] = "You already predicted this match!";
                this.TempData["ToastType"] = "danger";
                return this.RedirectToAction("Index", "Match");
            }

            var model = await this.predictionService.GetPredictionFormAsync(matchId);

            if (model == null)
            {
                return this.NotFound();
            }

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PredictionCreateViewModel model)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (await this.predictionService.HasUserPredictedAsync(model.MatchId, userId))
            {
                this.TempData["Toast"] = "You already predicted this match!";
                this.TempData["ToastType"] = "danger";
                return this.RedirectToAction("Index", "Match");
            }

            if (!this.ModelState.IsValid)
            {
                var formModel = await this.predictionService.GetPredictionFormAsync(model.MatchId);

                if (formModel != null)
                {
                    formModel.PredictedHomeGoals = model.PredictedHomeGoals;
                    formModel.PredictedAwayGoals = model.PredictedAwayGoals;
                    model = formModel;
                }

                return this.View(model);
            }

            var success = await this.predictionService.CreateAsync(model, userId);

            if (!success)
            {
                this.TempData["Toast"] = "Cannot predict this match — it may already be finished.";
                this.TempData["ToastType"] = "danger";
                return this.RedirectToAction("Index", "Match");
            }

            this.TempData["Toast"] = "Prediction saved! 🎯";
            this.TempData["ToastType"] = "success";

            return this.RedirectToAction(nameof(Mine));
        }

        [HttpGet]
        public async Task<IActionResult> Mine(int page = 1)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = await this.predictionService.GetUserPredictionsPagedAsync(userId, page, 10);
            model.Stats = await this.predictionService.GetUserStatsAsync(userId);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await this.predictionService.DeleteAsync(id, userId);

            if (success)
            {
                this.TempData["Toast"] = "Prediction removed.";
                this.TempData["ToastType"] = "info";
            }
            else
            {
                this.TempData["Toast"] = "Prediction not found.";
                this.TempData["ToastType"] = "danger";
            }

            return this.RedirectToAction(nameof(Mine));
        }
    }
}