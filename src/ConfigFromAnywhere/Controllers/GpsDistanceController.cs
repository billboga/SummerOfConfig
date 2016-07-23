using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Threading.Tasks;

namespace ConfigFromAnywhere.Controllers
{
    public class GpsDistanceController : Controller
    {
        public GpsDistanceController(IConfigurationRoot config)
        {
            this.config = config;
        }

        private readonly IConfigurationRoot config;

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            var coordinates = await Request.ReadFormAsync();

            Startup.DistancePositionChanged(
                double.Parse(coordinates["latitude"]),
                double.Parse(coordinates["longitude"]),
                coordinates["accuracy"]);

            return new StatusCodeResult((int)HttpStatusCode.Accepted);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
