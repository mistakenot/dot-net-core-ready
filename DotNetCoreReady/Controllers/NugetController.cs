using System.Linq;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using System.Web.Mvc;
using System.Web.UI;
using DotNetCoreReady.Extensions;
using DotNetCoreReady.Services;
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

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
        [OutputCache(Location = OutputCacheLocation.Server, Duration = 36000, VaryByParam = "searchTerm")]
        public async Task<JsonResult> Autocomplete(string searchTerm)
        {
            var suggestions = await _client.Autocomplete(searchTerm);
            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.Server, Duration = 36000, VaryByParam = "searchTerm")]
        public async Task<JsonResult> Search(string searchTerm)
        {
            var searchMetadata = await _client.Search(searchTerm, false);

            var models = searchMetadata
                .Select(s => s.ToLookupViewModel())
                .ToArray();

            return Json(models, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.Server, Duration = 36000, VaryByParam = "id")]
        public async Task<JsonResult> Frameworks(string id)
        {
            var versions = await _client.FindLatestVersions(id);
            var models = versions.Select(v => v.ToLookupViewModel());

            return Json(models, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.Server, Duration = 36000, VaryByParam = "id")]
        public async Task<JsonResult> Alternatives(string id)
        {
            var searchResponse = await _client.Alternatives(id);
            var response = searchResponse
                .Select(s => s.ToViewModel())
                .ToArray();

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.Server, Duration = 36000, VaryByParam = "packageId;version")]
        public async Task<JsonResult> Dependencies(
            [QueryString]string packageId,
            [QueryString]string version = null)
        {
            IPackageSearchMetadata metadata;

            if (string.IsNullOrEmpty(packageId))
            {
                return Json(new {Error = true, Msg = "Package ID is invalid."}, JsonRequestBehavior.AllowGet);
            }

            NuGetVersion nugetVersion;
            if (!string.IsNullOrEmpty(version) && !NuGetVersion.TryParse(version, out nugetVersion))
            {
                return Json(new {Error = true, Msg = "Package version is invalid."}, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(version))
            {
                var searchResults = await _client.FindLatestVersions(packageId, 1);
                metadata = searchResults.FirstOrDefault();
            }
            else
            {
                metadata = await _client.FindVersion(packageId, version);
            }
            
            var deps = (metadata != null ? metadata.DependencySets : Enumerable.Empty<PackageDependencyGroup>())
                .Select(ds => new
                {
                    Framework = ds.TargetFramework.ToString(),
                    Dependencies = ds.Packages.Select(p => new
                    {
                        p.Id,
                        Version = p.VersionRange.ToShortString(),
                        Url = Url.Action("Dependencies", "Nuget", new { packageId = p.Id, version = p.VersionRange.ToShortString() })
                    })
                })
                .ToArray();

            return Json(deps, JsonRequestBehavior.AllowGet);
        }
    }
}