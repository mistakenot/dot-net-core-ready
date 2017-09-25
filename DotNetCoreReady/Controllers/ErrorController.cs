using System.Web.Mvc;

namespace DotNetCoreReady.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet]
        public ActionResult Throw(string msg = "this_is_a_error")
        {
            throw new System.Exception(msg);
        }
    }
}