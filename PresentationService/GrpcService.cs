using DDD;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PresentationService.API;
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
using Utilities.Extensions;
using Shared.Types;

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

        public override async Task<GListTestsResponse> ListTests(GListTestsRequest gRequest, ServerCallContext context)
        {
            ListTestsRequest request = gRequest;

            var tests = await TestsStorageService.ListTestsDataAsync(new GListTestsDataRequest()
            {
                ByRange = new GRange() 
                { 
                    From = request.Range.From, 
                    To = request.Range.To 
                },
                IncludeData = false,
            });
            var ff = JsonConvert
                .DeserializeObject<TestsStorageService.Db.TestCase[]>(tests.Tests.ToStringUtf8())
                .OrderBy(t => t.TestId);

            var testsIds = ff.Select(c => c.TestId).ToArray();
            var getInfosR = new RunnerService.API.GetTestsInfoRequest(testsIds);
            RunnerService.API.GetTestsInfoResponse getInfosResp = await RunnerService.GetTestsInfoAsync(getInfosR);

            var fullInfos = ff.Zip(getInfosResp.RunInfos, (Case, RunInfo) => (Case, RunInfo));

            return new ListTestsResponse(ddd().ToArray(), tests.Count.ToInt32(), Protobuf.StatusCode.Ok);

            IEnumerable<TestInfo> ddd()
            {
                foreach (var info in fullInfos)
                {
                    yield return new TestInfo()
                    {
                        TestId = info.Case.TestId,
                        Author = new GetUserInfoResponse(info.Case.AuthorName, null, null, Protobuf.StatusCode.Ok),
                        Target = new CSTestCaseInfo() {  DisplayName = info.Case.DisplayName, TargetType = info.Case?.Data?.Type },
                        State = info.RunInfo.State,
                        LastResult = info.RunInfo.LastResult,
                        RunPlan = info.RunInfo.RunPlan,
                        CreationState = info.Case.State
                    };
                }
            }
        }

        public override async Task<GBeginRecordingResponse> BeginAddTest(GBeginRecordingRequest gRequest, ServerCallContext context)
        {
            BeginAddTestRequest request = gRequest;
            var token = context.RequestHeaders.FirstOrDefault(h => h.Key == "token")?.Value ?? "";

            var result = await UserService.ValidateTokenAsync(new GValidateTokenRequest() { Token = token });
            if (result.Valid)
            {
                var userInfResp = await UserService.GetUserInfoAsync(new  GGetUserInfoRequest() { Token = token });

                var crTestReq = new GTryCreateTestRequest()
                {
                    Author = userInfResp.UserName,
                    DisplayName = request.DisplayName,
                    TestId = request.TestId
                };
                var crTestResp = await TestsStorageService.TryCreateTestAsync(crTestReq);
                if (crTestResp.IsAlreadyAdded)
                {
                    return new BeginAddTestResponse(new ResponseStatus(Protobuf.StatusCode.Error, $"Test with id \"{request.TestId}\" already exists"));
                }
                else
                {
                    var beginRecReq = new TestsSourceService.API.GBeginRecordingRequest();
                    beginRecReq.Filter.Add(request.TestParameters);
                    beginRecReq.TestId = request.TestId;
                    var recResult = await TestsSourceService.BeginRecordingAsync(beginRecReq);

                    return new BeginAddTestResponse(recResult.Status);
                }
            }
            else
            {
                return new BeginAddTestResponse(Protobuf.StatusCode.NotAuthorized);
            }
        }

        public override async Task<GRunTestResponse> BeginTest(GRunTestRequest gRequest, ServerCallContext context)
        {
            BeginTestRequest request = gRequest;
            var token = context.RequestHeaders.FirstOrDefault(h => h.Key == "token")?.Value ?? "";

            var result = await UserService.ValidateTokenAsync(new GValidateTokenRequest() { Token = token });
            if (result.Valid)
            {
                var userInfResp = await UserService.GetUserInfoAsync(new GGetUserInfoRequest() { Token = token });

                var runReq = new RunnerService.API.GRunTestRequest();
                runReq.TestId = request.TestId;
                runReq.UserName = userInfResp.UserName;
                var runResponse = await RunnerService.RunTestAsync(runReq);

                return new BeginTestResponse(runResponse.Status);
            }
            else
            {
                return new BeginTestResponse(Protobuf.StatusCode.NotAuthorized);
            }
        }

        public override async Task<GGetTestDetailsResponse> GetTestDetails(GGetTestDetailsRequest gRequest, ServerCallContext context)
        {
            return await RunnerService.GetTestDetailsAsync(gRequest);
        }
    }
}
