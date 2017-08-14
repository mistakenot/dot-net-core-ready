using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using DotNetCoreReady.Extensions;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace DotNetCoreReady.Controllers
{
    public class NugetController : Controller
    {
        [HttpGet]
        public async Task<JsonResult> Search(string searchTerm)
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
        public async Task<JsonResult> Frameworks(string id, string version)
        {
            var packageId = new PackageIdentity(id, NuGetVersion.Parse(version));
            var resource = await GetResource<PackageMetadataResource>();
            var metadata = await resource.GetMetadataAsync(packageId, new NullLogger(), CancellationToken.None);
            var versions = metadata
                .DependencySets
                .Select(d => d.TargetFramework.ToViewModel())
                .Where(v => v.FrameworkId != ".NETPortable")
                .OrderByDescending(v => v.FrameworkId)
                .ThenByDescending(v => v.Version)
                .ToList();

            return Json(versions, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> Suggestions(string id, string version)
        {
            var packageId = new PackageIdentity(id, NuGetVersion.Parse(version));
            var resource = await GetResource<PackageMetadataResource>();
            var metadata = await resource.GetMetadataAsync(packageId, new NullLogger(), CancellationToken.None);
            var tags = metadata.Tags;
            var searchResource = await GetResource<PackageSearchResource>();
            var searchRequest = await searchResource.SearchAsync("", new SearchFilter(true), 0, 5, new NullLogger(), CancellationToken.None);
            var response = searchRequest.Select(s => s.ToViewModel());

            return Json(response);
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
    }
}