using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DotNetCoreReady.Extensions;

namespace DotNetCoreReady.Tests
{
    [TestClass]
    public class TaskCompleteTests
    {
        [TestMethod]
        public void WhenMOfNTasksAreComplete_ShouldComplete_Ok()
        {
            var tasks = Enumerable.Range(1, 5).Select(async i =>
            {
                await Task.Delay(i * 1000);
                return i;
            });

            var waitTask = tasks.WhenSome(2).Result;
            var results = waitTask.Select(t => t.Result).ToArray();

            Assert.AreEqual(2, waitTask.Count());
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(2, results[1]);
        }
    }
}
