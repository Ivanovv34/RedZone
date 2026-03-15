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
            var model = await predictionService.GetPredictionFormAsync(matchId);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PredictionCreateViewModel model)
        {
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

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await predictionService.CreateAsync(model, userId);

            return RedirectToAction(nameof(Mine));
        }

        public async Task<IActionResult> Mine()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = await predictionService.GetUserPredictionsAsync(userId);

            return View(model);
        }
    }
}