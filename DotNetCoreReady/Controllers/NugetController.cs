using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DotNetCoreReady.Extensions;
using DotNetCoreReady.Models;
using DotNetCoreReady.Services;

namespace DotNetCoreReady.Controllers
{
    public class NugetController : Controller
    {
        private readonly NugetClient _client;

        public NugetController()
        {
            _client = new NugetClient();
        }

        [HttpGet]
        public async Task<JsonResult> Autocomplete(string searchTerm)
        {
            var suggestions = await _client.Autocomplete(searchTerm);
            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> Search(string searchTerm)
        {
            var searchMetadata = await _client.Search(searchTerm, false);

            var models = searchMetadata
                .Select(s => s.ToLookupViewModel())
                .ToArray();

            return Json(models, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> Frameworks(string id)
        {
            var versions = await _client.FindLatestVersions(id);
            var models = versions.Select(v => v.ToLookupViewModel());

            return Json(models, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> Alternatives(string id)
        {
            var searchResponse = await _client.Alternatives(id);
            var response = searchResponse
                .Select(s => s.ToViewModel())
                .ToArray();

            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}