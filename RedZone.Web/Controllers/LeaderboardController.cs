using Microsoft.AspNetCore.Mvc;
using RedZone.Services.Core.Interfaces;

namespace RedZone.Web.Controllers
{
    public class LeaderboardController : Controller
    {
        private readonly IPredictionService predictionService;

        public LeaderboardController(IPredictionService predictionService)
        {
            this.predictionService = predictionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await this.predictionService.GetLeaderboardAsync();
            return this.View(model);
        }
    }
}