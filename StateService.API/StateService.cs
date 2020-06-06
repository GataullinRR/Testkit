using Shared.Types;
using System.Net.Http;
using System.Threading.Tasks;

namespace StateService.API
{
    public class StateService : ServiceBase, IStateService
    {
        public StateService(HttpClient client) : base(client)
        {

        }

        public Task<GetTestsAddStateResponse> GetTestsAddStateAsync(GetTestsAddStateRequest request)
        {
            return MakeRequestAsync<GetTestsAddStateRequest, GetTestsAddStateResponse>(HttpMethod.Post, nameof(GetTestsAddStateAsync), request);
        }
    }
}
