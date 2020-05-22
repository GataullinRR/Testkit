using DDD;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PresentationService.API2;
using RunnerService.APIModels;
using RunnerService.Db;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestsStorageService.API;
using UserService.API;
using Utilities.Types;

namespace PresentationService
{
    public class GrpcService : API2.PresentationService.PresentationServiceBase
    {
        [Inject] public UserService.API.UserService.UserServiceClient UserService { get; set; }
        [Inject] public TestsStorageService.API.TestsStorageService.TestsStorageServiceClient TestsStorageService { get; set; }
        [Inject] public TestsSourceService.API.TestsSourceService.TestsSourceServiceClient TestsSourceService { get; set; }
        [Inject] public RunnerService.API.RunnerService.RunnerServiceClient RunnerService { get; set; }
        [Inject] public IHubContext<SignalRHub, IMainHub> Hub { get; set; }
        [Inject] public ILogger<GrpcService> Logger { get; set; }
        [Inject] public JsonSerializerSettings JsonSettings { get; set; }

        public GrpcService(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        public override async Task<ListTestsResponse> ListTests(ListTestsRequest request, ServerCallContext context)
        {
            Logger.LogTrace("ListRequest");

            //await Hub.Clients.All.TestRecorded(new API.TestRecordedWebMessage() { DisplayName = "HELLO!" }); // for test

            var response = new ListTestsResponse()
            {
                Status = new Protobuf.ResponseStatus()
            };

            var tests = await TestsStorageService.ListTestsDataAsync(new ListTestsDataRequest()
            {
                ByRange = request.Range,
                IncludeData = false,
            });

            var ff = JsonConvert
                .DeserializeObject<TestCase[]>(tests.Tests.ToStringUtf8())
                .OrderBy(t => t.Id);

            var testsIds = ff.Select(c => c.Id).ToArray();
            var getInfosR = new RunnerService.API.GetTestsInfoRequest();
            getInfosR.TestsIds.AddRange(testsIds);
            var getInfosResp = await RunnerService.GetTestsInfoAsync(getInfosR);
            var infos = JsonConvert
                .DeserializeObject<TestRunInfo[]>(getInfosResp.Infos.ToStringUtf8(), JsonSettings)
                .OrderBy(i => i.TestId);

            var fullInfos = ff.Zip(infos, (Case, RunInfo) => (Case, RunInfo));

            var testInfos = ddd().ToArray();
            string json = JsonConvert.SerializeObject(testInfos, JsonSettings);

            response.TotalCount = tests.Count;
            response.TestInfos = ByteString.CopyFromUtf8(json);

            IEnumerable<TestInfo> ddd()
            {
                foreach (var info in fullInfos)
                {
                    yield return new TestInfo()
                    {
                        TestId = info.Case.Id,
                        Author = new CSUserInfo() { UserName = info.Case.AuthorName },
                        Target = info.Case.CaseInfo,
                        State = info.RunInfo.State,
                        LastResult = info.RunInfo.LastRun,
                        RunPlan = info.RunInfo.RunPlan
                    };
                }
            }

            return response;
        }

        public override async Task<BeginRecordingResponse> BeginRecording(BeginRecordingRequest request, ServerCallContext context)
        {
            Logger.LogTrace("BeginRecordingRequest");

            var response = new BeginRecordingResponse()
            {
                Status = new Protobuf.ResponseStatus()
            };

            var result = await UserService.ValidateTokenAsync(new ValidateTokenRequest() { Token = request.Token });
            if (result.Valid)
            {
                var beginRecRequet = new TestsSourceService.API.BeginRecordingRequest();
                beginRecRequet.Filter.Add(request.Filter);
                var recResult = await TestsSourceService.BeginRecordingAsync(beginRecRequet);

                response.Status = recResult.Status;
            }
            else
            {
                response.Status.Code = Protobuf.StatusCode.NotAuthorized;
            }

            return response;
        }

        public override async Task<RunTestResponse> RunTest(RunTestRequest request, ServerCallContext context)
        {
            var runResponse = await RunnerService.RunTestAsync(new RunnerService.API.RunTestRequest() { TestId = request.TestId });

            return new RunTestResponse()
            {
                Status = new Protobuf.ResponseStatus()
            };
        }
    }
}
