using DotNetCoreReady.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetCoreReady.Tests.Controllers
{
    [TestClass]
    public class NugetClientTests
    {
        private readonly NugetClient _client;

        public NugetClientTests()
        {
            _client = new NugetClient();
        }

        [TestMethod]
        public void Get_alternatives_get_standard_frameworks()
        {
            _client.Search("Newtonsoft.Json", true).Wait();
        }
    }
}