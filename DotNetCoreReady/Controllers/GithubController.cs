using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DotNetCoreReady.Extensions;
using Octokit;

namespace DotNetCoreReady.Controllers
{
    public class GithubController : Controller
    {
        private readonly GitHubClient _client;

        public GithubController()
        {
            var productHeaderValue = new ProductHeaderValue("DotNetCoreReady");
            _client = new GitHubClient(productHeaderValue);
        }

        [HttpGet]
        public async Task<ActionResult> Search(string searchTerm, string url = null)
        {
            Repository repoFoundByUrl = null;

            if (!string.IsNullOrEmpty(url))
            {
                var uri = new Uri(url);

                if (uri.Segments.Length == 2)
                {
                    repoFoundByUrl = await _client.Repository.Get(uri.Segments[0], uri.Segments[1]);
                }
            }

            SearchIssuesRequest searchIssuesRequest = null;

            if (repoFoundByUrl != null)
            {
                var repoCollection = new RepositoryCollection();
                repoCollection.Add(repoFoundByUrl.Owner.Name, repoFoundByUrl.Name);

                searchIssuesRequest = new SearchIssuesRequest(".NET Core Standard")
                {
                    Repos = repoCollection,
                    Type = IssueTypeQualifier.Issue,
                    Language = Language.CSharp,
                    In = new List<IssueInQualifier> { IssueInQualifier.Title }
                };
            }
            else
            {
                var findRepoRequest = new SearchRepositoriesRequest(searchTerm)
                {
                    SortField = RepoSearchSort.Stars,
                    In = new[] {InQualifier.Name},
                    Language = Language.CSharp
                };

                var findRepoResponse = await _client.Search.SearchRepo(findRepoRequest);
                var repoCollection = new RepositoryCollection();

                foreach (var repoResponse in findRepoResponse.Items.Take(1))
                {
                    repoCollection.Add(repoResponse.Owner.Login, repoResponse.Name);
                }

                searchIssuesRequest = new SearchIssuesRequest(".NET Core Standard")
                {
                    Repos = repoCollection,
                    Type = IssueTypeQualifier.Issue,
                    Language = Language.CSharp,
                    State = ItemState.Open
                };
            }

            //var response = new[]
            //{
            //    new GithubIssueModel() {IsOpen = true, Title = "A title", Url = "http://github.com", Summary = "This is a comment summary"},
            //    new GithubIssueModel() {IsOpen = false, Title = "A title", Url = "http://github.com", Summary = "This is a comment summary"},
            //    new GithubIssueModel() {IsOpen = false, Title = "A title", Url = "http://github.com", Summary = "This is a comment summary"}
            //};

            //return Json(response, JsonRequestBehavior.AllowGet);

            var issuesSearchResult = await _client.Search.SearchIssues(searchIssuesRequest);
            var response = issuesSearchResult.Items.Select(r => r.ToViewModel());

            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}