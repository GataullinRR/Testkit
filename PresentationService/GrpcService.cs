using DDD;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PresentationService.API2;
using Protobuf;
using RunnerService.APIModels;
using RunnerService.Db;
using System;
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

            //await Hub.Clients.Group(request.Token).TestRecorded(new API.TestRecordedWebMessage() { DisplayName = "Msg by token!" }); // for test

            var uInfReq = new GetUserInfoRequest();
            uInfReq.Token = request.Token;
            var uInfResp = await UserService.GetUserInfoAsync(uInfReq);

            //await Hub.Clients.Group(uInfResp.UserName).TestRecorded(new API.TestRecordedWebMessage() { DisplayName = $"Msg by name! {uInfResp.UserName}" }); // for test
            //await Hub.Clients.Group(uInfResp.UserName).TestCompleted2(new API.TestCompletedWebMessage() { TestId = $"Msg by name! {uInfResp.UserName}" , 
            //     RunResult = new PassedResult() }); // for test
            //await Hub.Clients.All.TestCompleted2(new API.TestCompletedWebMessage() { TestId = $"Msg by name! {uInfResp.UserName}" , RunResult = new PassedResult() }); // for test

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
                .DeserializeObject<TestsStorageService.Db.TestCase[]>(tests.Tests.ToStringUtf8())
                .OrderBy(t => t.TestId);

            var testsIds = ff.Select(c => c.TestId).ToArray();
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
                        TestId = info.Case.TestId,
                        Author = new CSUserInfo() { UserName = info.Case.AuthorName },
                        Target = new CSTestCaseInfo() {  DisplayName = info.Case.DisplayName, TargetType = info.Case?.Data?.Type },
                        State = info.RunInfo.State,
                        LastResult = info.RunInfo.LastRun,
                        RunPlan = info.RunInfo.RunPlan,
                        CreationState = info.Case.State
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
                Status = new ResponseStatus()
            };

            var result = await UserService.ValidateTokenAsync(new ValidateTokenRequest() { Token = request.Token });
            if (result.Valid)
            {
                var userInfResp = await UserService.GetUserInfoAsync(new  GetUserInfoRequest() { Token = request.Token });

                var crTestReq = new TryCreateTestRequest()
                {
                    Author = userInfResp.UserName,
                    DisplayName = request.DisplayName,
                    TestId = request.TestId
                };
                var crTestResp = await TestsStorageService.TryCreateTestAsync(crTestReq);
                if (crTestResp.IsAlreadyAdded)
                {
                    response.Status.Code = Protobuf.StatusCode.Error;
                    response.Status.Description = $"Test with id \"{request.TestId}\" already exists";
                }
                else
                {
                    var beginRecReq = new TestsSourceService.API.BeginRecordingRequest();
                    beginRecReq.Filter.Add(request.Filter);
                    beginRecReq.TestId = request.TestId;
                    var recResult = await TestsSourceService.BeginRecordingAsync(beginRecReq);

                    response.Status = recResult.Status;
                }
            }
            else
            {
                response.Status.Code = Protobuf.StatusCode.NotAuthorized;
            }

            return response;
        }

        public override async Task<RunTestResponse> RunTest(RunTestRequest request, ServerCallContext context)
        {
            var response = new RunTestResponse()
            {
                Status = new ResponseStatus()
            };

            var result = await UserService.ValidateTokenAsync(new ValidateTokenRequest() { Token = request.Token });
            if (result.Valid)
            {
                //var userInfResp = await UserService.GetUserInfoAsync(new GetUserInfoRequest() { Token = request.Token });

                var runReq = new RunnerService.API.RunTestRequest();
                runReq.TestId = request.TestId;
                var runResponse = await RunnerService.RunTestAsync(runReq);

                response.Status.Code = runResponse.Status.Code;
            }
            else
            {
                response.Status.Code = Protobuf.StatusCode.NotAuthorized;
            }

            return response;
        }
    }
}
