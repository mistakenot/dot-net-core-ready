using System.Linq;
using System.Threading.Tasks;
using DotNetCoreReady.Extensions;
using DotNetCoreReady.Services;
using Microsoft.AspNetCore.Mvc;
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
        // [OutputCache(Location = OutputCacheLocation.Server, Duration = 36000, VaryByParam = "searchTerm")]
        public async Task<JsonResult> Autocomplete(string searchTerm)
        {
            var suggestions = await _client.Autocomplete(searchTerm);
            return Json(suggestions);
        }

        [HttpGet]
        // [OutputCache(Location = OutputCacheLocation.Server, Duration = 36000, VaryByParam = "searchTerm")]
        public async Task<JsonResult> Search(string searchTerm)
        {
            var searchMetadata = await _client.Search(searchTerm, false);

            var models = searchMetadata
                .Select(s => s.ToLookupViewModel())
                .ToArray();

            return Json(models);
        }

        [HttpGet]
        // [OutputCache(Location = OutputCacheLocation.Server, Duration = 36000, VaryByParam = "id")]
        [ResponseCacheAttribute(VaryByQueryKeys=new[] {"id"})]
        public async Task<JsonResult> Frameworks(string id)
        {
            var versions = await _client.FindLatestVersions(id);
            var models = versions.Select(v => v.ToLookupViewModel());

            return Json(models);
        }

        [HttpGet]
        // [OutputCache(Location = OutputCacheLocation.Server, Duration = 36000, VaryByParam = "id")]
        public async Task<JsonResult> Alternatives(string id)
        {
            var searchResponse = await _client.Alternatives(id);
            var response = searchResponse
                .Select(s => s.ToViewModel())
                .ToArray();

            return Json(response);
        }

        [HttpGet]
        // [OutputCache(Location = OutputCacheLocation.Server, Duration = 36000, VaryByParam = "packageId;version")]
        [ResponseCacheAttribute(VaryByQueryKeys=new[] {"packageId", "version"})]
        public async Task<JsonResult> Dependencies(
            string packageId,
            string version = null)
        {
            IPackageSearchMetadata metadata;

            if (string.IsNullOrEmpty(packageId))
            {
                return Json(new {Error = true, Msg = "Package ID is invalid."});
            }

            NuGetVersion nugetVersion;
            if (!string.IsNullOrEmpty(version) && !NuGetVersion.TryParse(version, out nugetVersion))
            {
                return Json(new {Error = true, Msg = "Package version is invalid."});
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

            return Json(deps);
        }
    }
}