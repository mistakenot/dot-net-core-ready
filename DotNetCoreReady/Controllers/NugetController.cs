using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using DotNetCoreReady.Extensions;
using DotNetCoreReady.Models;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace DotNetCoreReady.Controllers
{
    public class NugetController : Controller
    {
        [HttpGet]
        public async Task<JsonResult> Autocomplete(string searchTerm)
        {
            var searchResource = await GetResource<PackageSearchResource>();
            var searchMetadata = await searchResource.SearchAsync(
                searchTerm,
                new SearchFilter(true), 0, 5, null, CancellationToken.None);

            var models = searchMetadata
                .Select(s => s.Identity.Id)
                .ToArray();

            return Json(models, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> Search(string searchTerm)
        {
            var searchMetadata = await SearchInternal(searchTerm);

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
            var searchRequest = await SearchInternal(tags, true);
            var response = searchRequest
                .Select(s => s.ToViewModel())
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

        private static async Task<IEnumerable<IPackageSearchMetadata>> SearchInternal(
            string searchTerm,
            bool netStandardOnly = false)
        {
            var filter = new SearchFilter(true);

            if (netStandardOnly)
                filter.SupportedFrameworks = new[] { ".NETStandard" };

            var searchResource = await GetResource<PackageSearchResource>();
            var searchMetadata = await searchResource.SearchAsync(
                searchTerm,
                new SearchFilter(true), 0, 5, new NullLogger(), CancellationToken.None);

            return searchMetadata;
        }
    }
}