using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

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
            var searchResource = _sourceRepository.GetResource<PackageSearchResource>();
            var searchMetadata = await searchResource.SearchAsync(
                term,
                new SearchFilter(true), 0, 5, null, CancellationToken.None);

            var models = searchMetadata
                .Select(s => s.Identity.Id)
                .ToArray();

            return models;
        }

        public async Task<IEnumerable<IPackageSearchMetadata>> GetPackageVersionsById(string id)
        {
            var resource = _sourceRepository.GetResource<PackageMetadataResource>();
            var metadata = await resource.GetMetadataAsync(id, true, false, new NullLogger(), CancellationToken.None);

            return metadata.OrderByDescending(p => p.Identity.Version);
        }

        public Task<IEnumerable<IPackageSearchMetadata>> Search(string term, bool netStandardOnly)
        {
            return SearchInternal(term, netStandardOnly);
        }

        public async Task<IEnumerable<string>> GetGithubUrl(string packageId)
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
                    .Where(u => u.ToString().Contains("github") && u.Segments.Length > 1)
                    .Select(u => $"{u.Authority}/{u.Segments[0]}/{u.Segments[1]}");

                return urls;
            }

            return Enumerable.Empty<string>();
        }

        private async Task<IEnumerable<IPackageSearchMetadata>> SearchInternal(
            string searchTerm,
            bool netStandardOnly = false)
        {
            var filter = new SearchFilter(true);

            if (netStandardOnly)
                filter.SupportedFrameworks = new[] { ".NETStandard" };

            var searchResource = _sourceRepository.GetResource<PackageSearchResource>();
            var searchMetadata = await searchResource.SearchAsync(
                searchTerm,
                new SearchFilter(true), 0, 5, new NullLogger(), CancellationToken.None);

            return searchMetadata;
        }
    }
}