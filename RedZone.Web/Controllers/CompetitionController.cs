using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedZone.Services.Core.Interfaces;
using RedZone.ViewModels.Competition;

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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CompetitionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await competitionService.CreateAsync(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await competitionService.GetByIdAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, CompetitionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingCompetition = await competitionService.GetByIdAsync(id);

            if (existingCompetition == null)
            {
                return NotFound();
            }

            await competitionService.EditAsync(id, model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await competitionService.GetByIdAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var existingCompetition = await competitionService.GetByIdAsync(id);

            if (existingCompetition == null)
            {
                return NotFound();
            }

            await competitionService.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}