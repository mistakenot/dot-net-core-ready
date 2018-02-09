using System.Threading.Tasks;

namespace DotNetCoreReady.Services
{
    public interface IEmailAlertsRepository
    {
        Task<CreateResult> CreateIfNotExists(string email, string packageId, bool optedInToMarketing);
    }

    public class CreateResult
    {
        public bool WasSuccessful { get; set; } = true;

        public int ErrorCode { get; set; } = -1;
    }
}