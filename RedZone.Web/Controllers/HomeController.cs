using Microsoft.AspNetCore.Mvc;
using RedZone.Web.Models;
using System.Diagnostics;

namespace RedZone.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [HttpGet]
        public IActionResult StatusCode(int statusCode)
        {
            if (statusCode == 404)
            {
                return this.View("NotFound");
            }

            return this.View("Error", new ErrorViewModel
            {
                RequestId = HttpContext.TraceIdentifier
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}