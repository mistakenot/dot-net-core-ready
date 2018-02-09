using System.Collections.Generic;
using System.Linq;
using DotNetCoreReady.Models;
using NuGet.Frameworks;
using NuGet.Protocol.Core.Types;

namespace DotNetCoreReady.Extensions
{
    public static class PackageExtensions
    {
        public static NugetPackageModel ToViewModel(this IPackageSearchMetadata package)
        {
            return new NugetPackageModel
            {
                Id = package.Identity.Id,
                Name = package.Title,
                Version = package.Identity.Version.ToString(),
                ProjectUrl = package.ProjectUrl?.ToString()
            };
        }

        public static NugetFrameworkVersionModel ToViewModel(this NuGetFramework nugetFramework)
        {
            return new NugetFrameworkVersionModel
            {
                RuntimeId = nugetFramework.DotNetFrameworkName.Split(',').First(),
                RuntimeVersionId = nugetFramework.DotNetFrameworkName.Split('=').ElementAt(1),
                VersionId = nugetFramework.Version.ToString()
            };
        }

        public static NugetPackageLookupResultModel ToLookupViewModel(this IPackageSearchMetadata package)
        {
            return new NugetPackageLookupResultModel
            {
                PackageVersion = package.Identity.Version.ToString(),
                NetStandardVersions = package
                    .DependencySets
                    .OrderByDescending(d => d.TargetFramework.Version)
                    .Select(d => d.TargetFramework.ToViewModel())
                    .Where(v => v.RuntimeId == ".NETStandard")
                    .Select(v => v.RuntimeVersionId)
            };
        }
    }

    public class NugetPackageLookupResultModel
    {
        public string PackageVersion { get; set; }
        public IEnumerable<string> NetStandardVersions { get; set; }
    }
}