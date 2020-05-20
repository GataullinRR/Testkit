using DDD;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using PresentationService.API2;
using RunnerService.API;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestsStorageService.API;
using UserService.API;
using Utilities.Types;

namespace PresentationService
{
    public class GRPCController : API2.PresentationService.PresentationServiceBase
    {
        [Inject] public UserService.API.UserService.UserServiceClient UserService { get; set; }
        [Inject] public TestsStorageService.API.TestsStorageService.TestsStorageServiceClient TestsStorageService { get; set; }

        public GRPCController(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        public override async Task<ListTestsResponse> ListTests(ListTestsRequest request, ServerCallContext context)
        {
            var response = new ListTestsResponse()
            {
                Status = new Protobuf.ResponseStatus()
            };

            var result = await UserService.ValidateTokenAsync(new ValidateTokenRequest() { Token = request.Token });


            //var result = await UserService.ValidateTokenAsync(new ValidateTokenRequest() { Token = request.Token });
            //if (result.Valid)
            //{
            var tests = await TestsStorageService.ListTestsDataAsync(new ListTestsDataRequest()
            {
                ByRange = request.Range,
                IncludeData = false,
            });

            var jset = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
            string json = JsonConvert.SerializeObject(ddd().ToArray(), jset);

            response.TotalCount = tests.Count;
            response.TestInfos = ByteString.CopyFromUtf8(json);

            IEnumerable<TestInfo> ddd()
            {
                var ff = JsonConvert.DeserializeObject<TestCase[]>(tests.Tests.ToStringUtf8());
                foreach (var f in ff)
                {
                    yield return new TestInfo()
                    {
                        TestId = f.Id,
                        Author = new CSUserInfo() { UserName = f.AuthorName },
                        Target = f.CaseInfo,
                        State = new SuspendedState(),
                        LastResult = new OkResult(),
                        RunPlan = new PeriodicRunPlan(),
                    };
                }
            }
            //}
            //else
            //{
            //    response.Status.Code = Protobuf.StatusCode.NotAuthorized;
            //}

            return response;
        }
    }
}
