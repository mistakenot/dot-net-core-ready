namespace DotNetCoreReady.Models
{
    public class NugetSearchResult
    {
        public string PackageId { get; set; }

        public string[] Runtimes { get; set; }

        public string GithubUrl { get; set; }
    }
}