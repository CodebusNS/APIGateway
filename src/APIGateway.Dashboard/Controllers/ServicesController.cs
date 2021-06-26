using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIGateway.Dashboard.Controllers
{
    public class ServicesController:Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
