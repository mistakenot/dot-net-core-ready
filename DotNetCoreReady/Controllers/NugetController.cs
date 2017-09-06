using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using DotNetCoreReady.Extensions;
using DotNetCoreReady.Models;
using DotNetCoreReady.Services;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

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
            var versions = await FindLatestVersions(id);
            var models = versions.Select(v => v.ToLookupViewModel());

            return Json(models, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> Alternatives(string id)
        {
            var latestVersions = await FindLatestVersions(id, 1);

            if (!latestVersions.Any())
            {
                return Json(Enumerable.Empty<NugetPackageModel>(), JsonRequestBehavior.AllowGet);
            }

            var tags = latestVersions.Select(v => v.Tags).First();
            var searchResponse = await _client.Search(tags, true);
            var response = searchResponse
                .Select(s => s.ToViewModel())
                .Where(s => s.Id != id)
                .ToList();

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        private static Task<T> GetResource<T>()
            where T : class, INuGetResource
        {
            var providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());
            var packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
            var sourceRepository = new SourceRepository(packageSource, providers);

            return sourceRepository.GetResourceAsync<T>();
        }

        private static async Task<IEnumerable<IPackageSearchMetadata>> FindLatestVersions(string id, int count = 8)
        {
            var resource = await GetResource<PackageMetadataResource>();
            var metadata = await resource.GetMetadataAsync(id, true, false, new NullLogger(), CancellationToken.None);
            var versions = metadata
                .OrderByDescending(p => p.Identity.Version)
                .Take(count);

            return versions;
        }
    }
}