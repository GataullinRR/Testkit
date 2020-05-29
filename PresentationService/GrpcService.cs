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
using Vectors;
using MessageHub;

namespace PresentationService
{
    public class GrpcService : API2.PresentationService.PresentationServiceBase
    {
        [Inject] public UserService.API.UserService.UserServiceClient UserService { get; set; }
        [Inject] public TestsStorageService.API.TestsStorageService.TestsStorageServiceClient TestsStorageService { get; set; }
        [Inject] public TestsSourceService.API.TestsSourceService.TestsSourceServiceClient TestsSourceService { get; set; }
        [Inject] public RunnerService.API.RunnerService.RunnerServiceClient RunnerService { get; set; }
        [Inject] public StateService.API.StateService.StateServiceClient StateService { get; set; }
        [Inject] public IHubContext<SignalRHub, IMainHub> Hub { get; set; }
        [Inject] public ILogger<GrpcService> Logger { get; set; }
        [Inject] public JsonSerializerSettings JsonSettings { get; set; }
        [Inject] public IMessageProducer MessageProducer { get; set; }

        public GrpcService(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        public override async Task<GListTestsResponse> ListTests(GListTestsRequest gRequest, ServerCallContext context)
        {
            ListTestsRequest request = gRequest;

            ListTestsDataRequest lstRequest = null;
            if (request.IsByIds)
            {
                lstRequest = new ListTestsDataRequest(request.TestIds, request.Range, false, request.ReturnNotSaved);
            }
            else if (request.IsByAuthorName)
            {
                lstRequest = new ListTestsDataRequest(request.AuthorName, request.Range, false, request.ReturnNotSaved);
            }
            else
            {
                var filters = request.TestNameFilter == null
                    ? new string[0]
                    : new string[] { request.TestNameFilter };
                lstRequest = new ListTestsDataRequest(filters, request.Range, false, request.ReturnNotSaved);
            }
            ListTestsDataResponse tests = await TestsStorageService.ListTestsDataAsync(lstRequest);

            var testsIds = tests.Tests.Select(c => c.TestName).ToArray();
            var getInfosR = new RunnerService.API.GetTestsInfoRequest(testsIds);
            RunnerService.API.GetTestsInfoResponse getInfosResp = request.ReturnNotSaved 
                ? (RunnerService.API.GetTestsInfoResponse)null
                : (RunnerService.API.GetTestsInfoResponse)await RunnerService.GetTestsInfoAsync(getInfosR);

            var fullInfos = tests.Tests.Zip(getInfosResp?.RunInfos ?? ((RunnerService.API.TestRunInfo)null).Repeat(tests.Tests.Length).ToArray() , (Case, RunInfo) => (Case, RunInfo));

             var response = new ListTestsResponse(ddd().ToArray(), tests.Tests.Length, Protobuf.StatusCode.Ok);

            return response;

            IEnumerable<TestInfo> ddd()
            {
                foreach (var info in fullInfos)
                {
                    yield return new TestInfo()
                    {
                        TestId = info.Case.TestId,
                        TestName = info.Case.TestName,
                        Author = new GetUserInfoResponse(info.Case.AuthorName, null, null, Protobuf.StatusCode.Ok),
                        Target = new TestCaseInfo() 
                        {  
                            DisplayName = info.Case.DisplayName, 
                            TargetType = info.Case?.Data?.Type, 
                            Parameters = info.Case?.Data?.Parameters,
                            CreateDate = info.Case?.CreationDate ?? default
                        },
                        State = info.RunInfo?.State,
                        LastResult = info.RunInfo?.LastResult,
                        RunPlan = info.RunInfo?.RunPlan,
                        CreationState = info.Case.State
                    };
                }
            }
        }

        public override async Task<GBeginAddTestResponse> BeginAddTest(GBeginAddTestRequest gRequest, ServerCallContext context)
        {
            BeginAddTestRequest request = gRequest;
            var token = context.RequestHeaders.FirstOrDefault(h => h.Key == "token")?.Value ?? "";

            var result = await UserService.ValidateTokenAsync(new GValidateTokenRequest() { Token = token });
            if (result.Valid)
            {
                var userInfResp = await UserService.GetUserInfoAsync(new GGetUserInfoRequest() { Token = token });

                MessageProducer.FireBeginAddTest(new BeginAddTestMessage(userInfResp.UserName, new Dictionary<string, string>(request.TestParameters)));

                return new BeginAddTestResponse(Protobuf.StatusCode.Ok);
            }
            else
            {
                return new BeginAddTestResponse(Protobuf.StatusCode.NotAuthorized);
            }
        }

        public override async Task<GStopAddTestResponse> StopAddTest(GStopAddTestRequest gRequest, ServerCallContext context)
        {
            StopAddTestRequest request = gRequest;
            var token = context.RequestHeaders.FirstOrDefault(h => h.Key == "token")?.Value ?? "";

            var result = await UserService.ValidateTokenAsync(new GValidateTokenRequest() { Token = token });
            if (result.Valid)
            {
                var userInfResp = await UserService.GetUserInfoAsync(new GGetUserInfoRequest() { Token = token });

                MessageProducer.FireStopAddTest(new StopAddTestMessage(userInfResp.UserName));

                return new StopAddTestResponse(Protobuf.StatusCode.Ok);
            }
            else
            {
                return new StopAddTestResponse(Protobuf.StatusCode.NotAuthorized);
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

                var runReq = new RunnerService.API.RunTestRequest(request.TestNameFilter.ToSequence().ToArray(), userInfResp.UserName);
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

        public override async Task<Protobuf.GDeleteTestResponse> DeleteTest(Protobuf.GDeleteTestRequest gRequest, ServerCallContext context)
        {
            DeleteTestRequest request = gRequest;
            var token = context.RequestHeaders.FirstOrDefault(h => h.Key == "token")?.Value ?? "";
            var result = await UserService.ValidateTokenAsync(new GValidateTokenRequest() { Token = token });
            if (result.Valid)
            {
                var userInfResp = await UserService.GetUserInfoAsync(new GGetUserInfoRequest() { Token = token });
                var userName = userInfResp.UserName;
                var testAuthor = (request.IsById
                    ? await getAuthorNameAsync(request.TestId, true) ?? await getAuthorNameAsync(request.TestId, false)
                    : await getAuthorNameAsync(request.TestNameFilter, true) ?? await getAuthorNameAsync(request.TestNameFilter, false)).AuthorName;
                if (userName == testAuthor)
                {
                    return await TestsStorageService.DeleteTestAsync(request);
                }
                else
                {
                    return new DeleteTestResponse(Protobuf.StatusCode.NotAuthorized);
                }
            }
            else
            {
                return new DeleteTestResponse(Protobuf.StatusCode.NotAuthorized);
            }
        }

        async Task<TestCase> getAuthorNameAsync(string testName, bool returnNotSaved)
        {
            var lstReq = new ListTestsDataRequest(new string[] { testName }, new IntInterval(0, 1), false, returnNotSaved);
            ListTestsDataResponse lstResp = await TestsStorageService.ListTestsDataAsync(lstReq);

            return lstResp.Tests.FirstElement();
        }

        async Task<TestCase> getAuthorNameAsync(int testId, bool returnNotSaved)
        {
            var lstReq = new ListTestsDataRequest(new int[] { testId }, new IntInterval(0, 1), false, returnNotSaved);
            ListTestsDataResponse lstResp = await TestsStorageService.ListTestsDataAsync(lstReq);

            return lstResp.Tests.FirstElement();
        }

        public override async Task<GGetTestsAddStateResponse> GetTestsAddState(GGetTestsAddStateRequest request, ServerCallContext context)
        {
            var token = context.RequestHeaders.FirstOrDefault(h => h.Key == "token")?.Value ?? "";
            var userInfResp = await UserService.GetUserInfoAsync(new GGetUserInfoRequest() { Token = token });

            var statusResponse = await StateService.GetTestsAddStateAsync(new StateService.API.GGetTestsAddStateRequest()
            {
                UserName = userInfResp.UserName
            });

            return new GGetTestsAddStateResponse()
            {
                Status = new GResponseStatus(),
                HasBegan = statusResponse.HasBegan
            };
        }

        public override async Task<GSaveRecordedTestResponse> SaveRecordedTest(GSaveRecordedTestRequest gRequest, ServerCallContext context)
        {
            SaveRecordedTestRequest request = gRequest;

            var token = context.RequestHeaders.FirstOrDefault(h => h.Key == "token")?.Value ?? "";
            var result = await UserService.ValidateTokenAsync(new GValidateTokenRequest() { Token = token });
            if (result.Valid)
            {
                var userInfResp = await UserService.GetUserInfoAsync(new GGetUserInfoRequest() { Token = token });

                var saveRequest = new GSaveTestRequest()
                {
                    Author = userInfResp.UserName,
                    DisplayName = request.TestDescription,
                    TestId = request.TestId,
                    TestName = request.TestName
                };
                var saveResponse = await TestsStorageService.SaveTestAsync(saveRequest);

                return new SaveRecordedTestResponse(saveResponse.Status);
            }
            else
            {
                return new SaveRecordedTestResponse(Protobuf.StatusCode.NotAuthorized);
            }
        }
    }
}
