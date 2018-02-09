using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreReady.Extensions;
using DotNetCoreReady.Services;
using Microsoft.AspNetCore.Mvc;
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
        [ResponseCacheAttribute(VaryByQueryKeys=new[] {"packageId"})]
        public async Task<ActionResult> Search(string packageId)
        {
            var githubUrls = await _nugetClient.GetGithubUrls(packageId);
            var url = githubUrls.FirstOrDefault();

            if (url == null)
            {
                return Json(Enumerable.Empty<GithubIssueModel>());
            }

            var owner = url.Segments[1].Trim('/');
            var name = url.Segments[2].Trim('/');

            var repoCollection = new RepositoryCollection {{owner, name}};

            var searchIssuesRequest = new SearchIssuesRequest(".NET Core Standard")
            {
                Repos = repoCollection,
                Type = IssueTypeQualifier.Issue,
                State = ItemState.Open,
                Page = 0,
                PerPage = 8
            };

            var issuesSearchResult = await _githubClient.Search.SearchIssues(searchIssuesRequest);
            var response = issuesSearchResult.Items.Select(r => r.ToViewModel());

            return Json(response);
        }
    }
}