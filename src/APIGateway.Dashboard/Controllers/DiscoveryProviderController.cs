using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace APIGateway.Dashboard.Controllers
{
    public class DiscoveryProviderController : Controller
    {
        private readonly ILogger<DashboardController> _logger;

        public DiscoveryProviderController(ILogger<DashboardController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
