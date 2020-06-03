using RunnerService.API;
using RunnerService.API.Models;
using Shared.Types;
using System.Net.Http;
using System.Threading.Tasks;

namespace RunnerService.API
{
    public class RunnerService : ServiceBase, IRunnerService
    {
        public RunnerService(HttpClient client) : base(client)
        {

        }

        public Task<GetTestDetailsResponse> GetTestDetailsAsync(GetTestDetailsRequest request)
        {
            return MakeRequestAsync<GetTestDetailsRequest, GetTestDetailsResponse>(HttpMethod.Post, nameof(GetTestDetailsAsync), request);
        }

        public Task<GetTestsInfoResponse> GetTestsInfoAsync(GetTestsInfoRequest request)
        {
            return MakeRequestAsync<GetTestsInfoRequest, GetTestsInfoResponse>(HttpMethod.Post, nameof(GetTestsInfoAsync), request);
        }

        public Task<RunTestResponse> RunTestAsync(RunTestRequest request)
        {
            return MakeRequestAsync<RunTestRequest, RunTestResponse>(HttpMethod.Post, nameof(RunTestAsync), request);
        }
    }
}
