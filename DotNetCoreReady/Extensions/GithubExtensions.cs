using Octokit;

namespace DotNetCoreReady.Extensions
{
    public static class GithubExtensions
    {
        public static GithubIssueModel ToViewModel(this Issue issue)
        {
            return new GithubIssueModel
            {
                Title = issue.Title,
                Url = issue.Url.ToString(),
                IsOpen = issue.State == ItemState.Open
            };
        }
    }
}