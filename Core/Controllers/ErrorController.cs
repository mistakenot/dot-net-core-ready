using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreReady.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet]
        public IActionResult Throw(string msg = "this_is_a_error")
        {
            throw new System.Exception(msg);
        }
    }
}