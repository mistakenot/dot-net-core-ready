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
                Url = issue.HtmlUrl.ToString(),
                IsOpen = issue.State == ItemState.Open,
                Body = issue.Body.Substring(0, 300),
                CreatedAt = issue.CreatedAt.DateTime
            };
        }
    }
}