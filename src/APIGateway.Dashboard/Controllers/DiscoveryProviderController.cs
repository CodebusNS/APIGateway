using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace APIGateway.Dashboard.Controllers
{
    public class DiscoveryProviderController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public DiscoveryProviderController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
