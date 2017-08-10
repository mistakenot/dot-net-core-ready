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
                FrameworkId = nugetFramework.DotNetFrameworkName.Split(',').First(),
                Version = nugetFramework.DotNetFrameworkName.Split('=').ElementAt(1)
            };
        }
    }

    
}