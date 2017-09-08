using DotNetCoreReady.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetCoreReady.Tests.Controllers
{
    [TestClass]
    public class NugetControllerTest
    {
        [TestMethod]
        public void NugetController_SearchPackages_Ok()
        {
            var controller = new NugetController();
            var result = controller.Search("Newtonsoft.Json");
            Assert.IsNotNull(result.Result);
        }

        [TestMethod]
        public void NugetController_GetFrameworks_Ok()
        {
            var controller = new NugetController();
            var result = controller.Frameworks("Newtonsoft.Json").Result;
        }
    }
}