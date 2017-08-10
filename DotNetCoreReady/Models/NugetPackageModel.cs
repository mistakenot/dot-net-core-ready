using System.Collections.Generic;

namespace DotNetCoreReady.Models
{
    public class NugetPackageModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ProjectUrl { get; set; }
        public string Version { get; set; }
    }
}