using System.Threading.Tasks;
using System.Web.Mvc;
using Octokit;

namespace DotNetCoreReady.Controllers
{
    public class GithubController : Controller
    {
        private readonly ProductHeaderValue _productHeaderValue;

        public GithubController()
        {
            _productHeaderValue = new ProductHeaderValue("DotNetCoreReady");
        }
    }
}