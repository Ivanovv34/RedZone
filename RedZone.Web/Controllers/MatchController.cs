using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await matchService.GetAllAsync(userId);
            return View(model);
        }

        [HttpGet]
        [Authorize]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        [HttpGet]
        public async Task<IActionResult> EnterResult(int id)
        {
            var match = await matchService.GetByIdAsync(id);

            if (match == null)
            {
                return NotFound();
            }

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
            if (id != model.MatchId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var match = await matchService.GetByIdAsync(id);

            if (match == null)
            {
                return NotFound();
            }

            await matchService.EnterResultAsync(id, model);

            TempData["Toast"] = "Match result entered successfully.";
            TempData["ToastType"] = "success";

            return RedirectToAction(nameof(Index));
        }
    }
}