using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCoreReady.Extensions;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace DotNetCoreReady.Services
{
    public class NugetClient
    {
        private readonly SourceRepository _sourceRepository;

        public NugetClient()
        {
            var providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3());
            var packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
            _sourceRepository = new SourceRepository(packageSource, providers);
        }

        public async Task<IEnumerable<string>> Autocomplete(string term)
        {
            var searchResource = await _sourceRepository.GetResourceAsync<PackageSearchResource>();
            var searchMetadata = await searchResource.SearchAsync(
                term,
                new SearchFilter(true), 0, 20, null, CancellationToken.None);
            
            var models = searchMetadata
                .OrderByDescending(s => s.DownloadCount)
                .Select(s => s.Identity.Id)
                .ToArray();

            return models;
        }

        public async Task<IEnumerable<IPackageSearchMetadata>> GetPackageVersionsById(string id)
        {
            var resource = _sourceRepository.GetResource<PackageMetadataResource>();
            var metadata = await resource.GetMetadataAsync(id, true, false, new NullLogger(), CancellationToken.None);
            
            try
            {
                return metadata.OrderByDescending(p => p.Identity.Version).ToList();
            }
            catch (ArgumentException e)
            {
                // Throws when looking up some older packages due to version issues.
                // https://github.com/NuGet/Home/issues/5935
                Trace.TraceError($"Versioning error looking up {id}");
                Trace.TraceError(e.ToString());
                return Enumerable.Empty<IPackageSearchMetadata>();
            }
        }

        public Task<IEnumerable<IPackageSearchMetadata>> Search(string term, bool netStandardOnly)
        {
            return SearchInternal(term, netStandardOnly);
        }

        public async Task<IEnumerable<IPackageSearchMetadata>> Alternatives(string id)
        {
            // At the moment, search functionality on nuget can't search by framework, so we
            //  have to pull a bunch of random search results and do a second query for each one.

            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var latestVersions = await FindLatestVersions(id);

            if (!latestVersions.Any())
            {
                return Enumerable.Empty<IPackageSearchMetadata>();
            }

            var tags = latestVersions.Select(v => v.Tags).First();
            var searchMetadata = await SearchInternal(tags, true, 25);

            var frameworkCheckTasks = searchMetadata
                .Where(s => string.Compare(s.Identity.Id, id, StringComparison.OrdinalIgnoreCase) != 0 &&
                            !s.Identity.Id.ToLower().Contains(id.ToLower()))
                .Select(async m =>
                {
                    var versions = await GetPackageVersionsById(m.Identity.Id);
                    return versions.Where(
                        v => v.DependencySets.Any(ds => ds.TargetFramework.Framework.StartsWith(".NETStandard")));
                });
            
            var results = await frameworkCheckTasks.WhenSome(5);

            searchMetadata = results
                .SelectMany(t => t.Result)
                .DistinctBy(s => s.Identity.Id)
                .OrderByDescending(s => s.DownloadCount)
                .Take(5)
                .ToList();

            return searchMetadata;
        }

        public async Task<IEnumerable<Uri>> GetGithubUrls(string packageId)
        {
            var versions = await SearchInternal(packageId);

            if (versions.Any())
            {
                var first = versions.First();
                var urls = new[]
                    {
                        first.IconUrl,
                        first.LicenseUrl,
                        first.ProjectUrl,
                        first.ReportAbuseUrl
                    }
                    .Where(u => u != null)
                    .Where(u => u.Authority.Contains("github") && u.Segments.Length > 2)
                    .Select(u => $@"https://github.com/{u.Segments[1]}{u.Segments[2]}")
                    .Select(u => new Uri(u))
                    .ToArray();

                return urls;
            }

            return Enumerable.Empty<Uri>();
        }

        public async Task<IEnumerable<IPackageSearchMetadata>> FindLatestVersions(string id, int count = 8)
        {
            var resource = await _sourceRepository.GetResourceAsync<PackageMetadataResource>();
            var metadata = await resource.GetMetadataAsync(id, true, false, new NullLogger(), CancellationToken.None);
            var versions = metadata
                .OrderByDescending(p => p.Identity.Version)
                .Take(count);

            return versions;
        }

        public async Task<IPackageSearchMetadata> FindVersion(string id, string version)
        {
            var resource = await _sourceRepository.GetResourceAsync<PackageMetadataResource>();
            var metadata = await resource.GetMetadataAsync(
                new PackageIdentity(id, NuGetVersion.Parse(version)),
                new NullLogger(), 
                CancellationToken.None);

            return metadata;
        }

        private async Task<IEnumerable<IPackageSearchMetadata>> SearchInternal(
            string searchTerm,
            bool netStandardOnly = false,
            int limit = 5)
        {
            var searchResource = await _sourceRepository.GetResourceAsync<PackageSearchResource>();
            var searchMetadata = await searchResource.SearchAsync(
                searchTerm,
                new SearchFilter(true), 0, limit, new NullLogger(), CancellationToken.None);

            if (netStandardOnly)
            {
                var frameworkCheckTasks = searchMetadata.Select(async m =>
                {
                    var versions = await GetPackageVersionsById(m.Identity.Id);
                    return versions.Where(
                        v => v.DependencySets.Any(ds => ds.TargetFramework.Framework.StartsWith(".NETStandard")));
                });

                var results = await Task.WhenAll(frameworkCheckTasks.ToArray());
                searchMetadata = results.SelectMany(_ => _);
            }

            return searchMetadata;
        }
    }
}