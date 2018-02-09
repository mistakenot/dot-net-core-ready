using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreReady.Controllers
{
    public class DependenciesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}