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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (await predictionService.HasUserPredictedAsync(matchId, userId))
            {
                TempData["Toast"] = "You already predicted this match!";
                TempData["ToastType"] = "danger";
                return RedirectToAction("Index", "Match");
            }

            var model = await predictionService.GetPredictionFormAsync(matchId);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(PredictionCreateViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (await predictionService.HasUserPredictedAsync(model.MatchId, userId))
            {
                TempData["Toast"] = "You already predicted this match!";
                TempData["ToastType"] = "danger";
                return RedirectToAction("Index", "Match");
            }

            if (!ModelState.IsValid)
            {
                var formModel = await predictionService.GetPredictionFormAsync(model.MatchId);
                if (formModel != null)
                {
                    formModel.PredictedHomeGoals = model.PredictedHomeGoals;
                    formModel.PredictedAwayGoals = model.PredictedAwayGoals;
                    model = formModel;
                }
                return View(model);
            }

            await predictionService.CreateAsync(model, userId);

            TempData["Toast"] = "Prediction saved! 🎯";
            TempData["ToastType"] = "success";

            return RedirectToAction(nameof(Mine));
        }

        public async Task<IActionResult> Mine()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await predictionService.GetUserPredictionsAsync(userId);
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await predictionService.DeleteAsync(id, userId);

            if (success)
            {
                TempData["Toast"] = "Prediction removed.";
                TempData["ToastType"] = "info";
            }
            else
            {
                TempData["Toast"] = "Prediction not found.";
                TempData["ToastType"] = "danger";
            }

            return RedirectToAction(nameof(Mine));
        }
    }
}