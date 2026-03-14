using Microsoft.AspNetCore.Mvc;
using RedZone.Services.Core.Interfaces;
using RedZone.ViewModels.Match;

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
            var model = await matchService.GetAllAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await matchService.GetByIdAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var competitions = await matchService.GetAllCompetitionsAsync();

            var model = new MatchCreateViewModel
            {
                Competitions = competitions
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MatchCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var competitions = await matchService.GetAllCompetitionsAsync();
                model.Competitions = competitions;
                return View(model);
            }

            await matchService.CreateAsync(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await matchService.GetForEditAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            var competitions = await matchService.GetAllCompetitionsAsync();
            model.Competitions = competitions;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, MatchEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var competitions = await matchService.GetAllCompetitionsAsync();
                model.Competitions = competitions;
                return View(model);
            }

            var existingMatch = await matchService.GetForEditAsync(id);

            if (existingMatch == null)
            {
                return NotFound();
            }

            await matchService.EditAsync(id, model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await matchService.GetForDeleteAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var existingMatch = await matchService.GetForDeleteAsync(id);

            if (existingMatch == null)
            {
                return NotFound();
            }

            await matchService.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}