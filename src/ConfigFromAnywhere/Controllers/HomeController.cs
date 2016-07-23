using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ConfigFromAnywhere.Controllers
{
    public class HomeController : Controller
    {
        private IConfigurationRoot _config;

        public HomeController(IConfigurationRoot config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            bool buttonIsDisabled;

            bool.TryParse(_config["ButtonIsDisabled"], out buttonIsDisabled);

            ViewBag.BackgroundColor = _config["BackgroundColor"] ?? "transparent";
            ViewBag.ButtonEnabled = !buttonIsDisabled;

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
