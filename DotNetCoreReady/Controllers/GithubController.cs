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
        public async Task<ActionResult> Search(string searchTerm)
        {
            var response = new[] {new GithubIssueModel() {IsOpen = true, Title = "A title", Url = "github.com"}};
            return Json(response, JsonRequestBehavior.AllowGet);

            var request = new SearchRepositoriesRequest(searchTerm)
            {
                Language = Language.CSharp
            };

            var result = await _client.Search.SearchRepo(request);
            var repos = new RepositoryCollection();

            foreach (var repo in result.Items)
            {
                repos.Add(repo.Owner.Name, repo.Name);
            }

            var searchIssuesRequest = new SearchIssuesRequest
            {
                Repos = repos,
            };

            var issuesSearchResult = await _client.Search.SearchIssues(searchIssuesRequest);
            //var response = issuesSearchResult.Items.Select(r => r.ToViewModel());

            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}