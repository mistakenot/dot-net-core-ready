using System;

namespace DotNetCoreReady.Extensions
{
    public class GithubIssueModel
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public bool IsOpen { get; set; }
        public string Summary { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}