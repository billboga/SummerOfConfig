using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ConfigFromAnywhere.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(IConfigurationRoot config)
        {
            this.config = config;
        }

        private IConfigurationRoot config;

        public IActionResult Index()
        {
            double progress;

            double.TryParse(config["gps.distance.progress"], out progress);

            ViewBag.Progress = (progress * 100).ToString("N0") ?? "0";
            ViewBag.Accuracy = config["gps.accuracy"] ?? "∞";

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
