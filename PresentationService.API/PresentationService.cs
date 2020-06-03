using Shared.Types;
using System.Net.Http;
using System.Threading.Tasks;
using TestsStorageService.API;

namespace PresentationService.API
{
    public class PresentationService : ServiceBase, IPresentationService
    {
        public PresentationService(HttpClient client) : base(client)
        {

        }

        public Task<StopAddTestResponse> BeginAddTestAsync(BeginAddTestRequest request)
        {
            return MakeRequestAsync<BeginAddTestRequest, StopAddTestResponse>(HttpMethod.Post, nameof(BeginAddTestAsync), request);
        }

        public Task<BeginTestResponse> BeginTestAsync(BeginTestRequest request)
        {
            return MakeRequestAsync<BeginTestRequest, BeginTestResponse>(HttpMethod.Post, nameof(BeginTestAsync), request);
        }

        public Task<DeleteTestResponse> DeleteTestAsync(DeleteTestRequest request)
        {
            return MakeRequestAsync<DeleteTestRequest, DeleteTestResponse>(HttpMethod.Delete, nameof(DeleteTestAsync), request);
        }

        public Task<GetTestDetailsResponse> GetTestDetailsAsync(GetTestDetailsRequest request)
        {
            return MakeRequestAsync<GetTestDetailsRequest, GetTestDetailsResponse>(HttpMethod.Post, nameof(GetTestDetailsAsync), request);
        }

        public Task<GetTestsAddStateResponse> GetTestsAddStateAsync(GetTestsAddStateRequest request)
        {
            return MakeRequestAsync<GetTestsAddStateRequest, GetTestsAddStateResponse>(HttpMethod.Post, nameof(GetTestsAddStateAsync), request);
        }

        public Task<ListTestsResponse> ListTestsAsync(ListTestsRequest request)
        {
            return MakeRequestAsync<ListTestsRequest, ListTestsResponse>(HttpMethod.Post, nameof(ListTestsAsync), request);
        }

        public Task<SaveRecordedTestResponse> SaveRecordedTestAsync(SaveRecordedTestRequest request)
        {
            return MakeRequestAsync<SaveRecordedTestRequest, SaveRecordedTestResponse>(HttpMethod.Post, nameof(SaveRecordedTestAsync), request);
        }

        public Task<StopAddTestResponse> StopAddTestAsync(StopAddTestRequest request)
        {
            return MakeRequestAsync<StopAddTestRequest, StopAddTestResponse>(HttpMethod.Post, nameof(StopAddTestAsync), request);
        }
    }
}
