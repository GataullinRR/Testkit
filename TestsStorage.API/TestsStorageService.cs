using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SharedT.Types;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Utilities.Types;

namespace TestsStorageService.API
{
    [Service(ServiceLifetime.Scoped)]
    public class TestsStorageService : ServiceBase, ITestsStorageService
    {
        public TestsStorageService(HttpClient client) : base(client)
        {

        }

        public Task<DeleteTestResponse> DeleteTestAsync(DeleteTestRequest request)
        {
            return MakeRequestAsync<DeleteTestRequest, DeleteTestResponse>(HttpMethod.Post, "delete", request);
        }

        public Task<ListTestsDataResponse> ListTestsDataAsync(ListTestsDataRequest request)
        {
            return MakeRequestAsync<ListTestsDataRequest, ListTestsDataResponse>(HttpMethod.Post, "list", request);
        }

        public Task<SaveTestResponse> SaveTestAsync(SaveTestRequest request)
        {
            return MakeRequestAsync<SaveTestRequest, SaveTestResponse>(HttpMethod.Post, "save", request);
        }
    }
}
