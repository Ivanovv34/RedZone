using Microsoft.AspNetCore.Mvc;
using RedZone.Services.Core.Interfaces;

namespace RedZone.Web.Controllers
{
    public class CompetitionController : Controller
    {
        private readonly ICompetitionService competitionService;

        public CompetitionController(ICompetitionService competitionService)
        {
            this.competitionService = competitionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await competitionService.GetAllAsync();
            return View(model);
        }
    }
}