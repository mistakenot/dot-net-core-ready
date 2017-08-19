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
                Body = (issue.Body.Length > 300) ? issue.Body.Substring(0, 300) : issue.Body,
                CreatedAt = issue.CreatedAt.DateTime
            };
        }
    }
}