using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DotNetCoreReady.Extensions;
using DotNetCoreReady.Services;
using Octokit;

namespace DotNetCoreReady.Controllers
{
    public class GithubController : Controller
    {
        private readonly GitHubClient _githubClient;
        private readonly NugetClient _nugetClient;

        public GithubController()
        {
            var productHeaderValue = new ProductHeaderValue("DotNetCoreReady");

            _githubClient = new GitHubClient(productHeaderValue);
            _nugetClient = new NugetClient();
        }

        [HttpGet]
        public async Task<ActionResult> Search(string packageId)
        {
            var githubUrls = await _nugetClient.GetGithubUrl(packageId);
            var url = githubUrls.FirstOrDefault();

            if (url == null)
            {
                return Json(Enumerable.Empty<GithubIssueModel>(), JsonRequestBehavior.AllowGet);
            }

            var findRepoResponse = await _githubClient.Search.SearchRepo(null);
            var repoCollection = new RepositoryCollection();

            foreach (var repoResponse in findRepoResponse.Items.Take(1))
            {
                repoCollection.Add(repoResponse.Owner.Login, repoResponse.Name);
            }

            var searchIssuesRequest = new SearchIssuesRequest(".NET Core Standard")
            {
                Repos = repoCollection,
                Type = IssueTypeQualifier.Issue,
                Language = Language.CSharp,
                State = ItemState.Open
            };
            

            //var response = new[]
            //{
            //    new GithubIssueModel() {IsOpen = true, Title = "A title", Url = "http://github.com", Summary = "This is a comment summary"},
            //    new GithubIssueModel() {IsOpen = false, Title = "A title", Url = "http://github.com", Summary = "This is a comment summary"},
            //    new GithubIssueModel() {IsOpen = false, Title = "A title", Url = "http://github.com", Summary = "This is a comment summary"}
            //};

            //return Json(response, JsonRequestBehavior.AllowGet);

            var issuesSearchResult = await _githubClient.Search.SearchIssues(searchIssuesRequest);
            var response = issuesSearchResult.Items.Select(r => r.ToViewModel());

            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}