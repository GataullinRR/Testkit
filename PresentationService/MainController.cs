using DDD;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PresentationService.API;
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
using Microsoft.AspNetCore.Mvc;

namespace PresentationService
{
    [ApiController, Microsoft.AspNetCore.Mvc.Route("api/v1")]
    public class MainController : ControllerBase
    {
        [Inject] public UserService.API.UserService.UserServiceClient UserService { get; set; }
        [Inject] public ITestsStorageService TestsStorage { get; set; }
        [Inject] public RunnerService.API.RunnerService.RunnerServiceClient RunnerService { get; set; }
        [Inject] public StateService.API.StateService.StateServiceClient StateService { get; set; }
        [Inject] public IHubContext<SignalRHub, IMainHub> Hub { get; set; }
        [Inject] public ILogger<MainController> Logger { get; set; }
        [Inject] public JsonSerializerSettings JsonSettings { get; set; }
        [Inject] public IMessageProducer MessageProducer { get; set; }

        public MainController(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.Route(nameof(IPresentationService.ListTestsAsync))]
        public async Task<ListTestsResponse> ListTests(ListTestsRequest request)
        {
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
            ListTestsDataResponse tests = await TestsStorage.ListTestsDataAsync(lstRequest);

            var testsIds = tests.Tests.Select(c => c.TestName).ToArray();
            var getInfosR = new RunnerService.API.GetTestsInfoRequest(testsIds);
            RunnerService.API.GetTestsInfoResponse getInfosResp = request.ReturnNotSaved 
                ? (RunnerService.API.GetTestsInfoResponse)null
                : (RunnerService.API.GetTestsInfoResponse)await RunnerService.GetTestsInfoAsync(getInfosR);

            var fullInfos = tests.Tests.Zip(getInfosResp?.RunInfos ?? ((RunnerService.API.TestRunInfo)null).Repeat(tests.Tests.Length).ToArray() , (Case, RunInfo) => (Case, RunInfo));

            var response = new ListTestsResponse(ddd().ToArray(), tests.Tests.Length);

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

        [HttpPost, Microsoft.AspNetCore.Mvc.Route(nameof(IPresentationService.BeginAddTestAsync))]
        public async Task<IActionResult> BeginAddTest(BeginAddTestRequest request)
        {
            var token = HttpContext.Request.Headers.FirstOrDefault(h => h.Key == "token").Value.FirstElementOrDefault() ?? "";

            var result = await UserService.ValidateTokenAsync(new GValidateTokenRequest() { Token = token });
            if (result.Valid)
            {
                var userInfResp = await UserService.GetUserInfoAsync(new GGetUserInfoRequest() { Token = token });

                MessageProducer.FireBeginAddTest(new BeginAddTestMessage(userInfResp.UserName, new Dictionary<string, string>(request.TestParameters)));

                return Ok(new BeginAddTestResponse());
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.Route(nameof(IPresentationService.StopAddTestAsync))]
        public async Task<IActionResult> StopAddTest(StopAddTestRequest request)
        {
            var token = HttpContext.Request.Headers.FirstOrDefault(h => h.Key == "token").Value.FirstElementOrDefault() ?? "";

            var result = await UserService.ValidateTokenAsync(new GValidateTokenRequest() { Token = token });
            if (result.Valid)
            {
                var userInfResp = await UserService.GetUserInfoAsync(new GGetUserInfoRequest() { Token = token });

                MessageProducer.FireStopAddTest(new StopAddTestMessage(userInfResp.UserName));

                return Ok(new StopAddTestResponse());
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.Route(nameof(IPresentationService.BeginTestAsync))]
        public async Task<IActionResult> BeginTest(BeginTestRequest request)
        {
            var token = HttpContext.Request.Headers.FirstOrDefault(h => h.Key == "token").Value.FirstElementOrDefault() ?? "";

            var result = await UserService.ValidateTokenAsync(new GValidateTokenRequest() { Token = token });
            if (result.Valid)
            {
                var userInfResp = await UserService.GetUserInfoAsync(new GGetUserInfoRequest() { Token = token });

                var runReq = new RunnerService.API.RunTestRequest(request.TestNameFilter.ToSequence().ToArray(), userInfResp.UserName);
                var runResponse = await RunnerService.RunTestAsync(runReq);

                return Ok(new BeginTestResponse());
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.Route(nameof(IPresentationService.GetTestDetailsAsync))]
        public async Task<GetTestDetailsResponse> GetTestDetails(GetTestDetailsRequest request)
        {
            return await RunnerService.GetTestDetailsAsync(request);
        }

        [HttpDelete, Microsoft.AspNetCore.Mvc.Route(nameof(IPresentationService.DeleteTestAsync))]
        public async Task<IActionResult> DeleteTest(API.DeleteTestRequest request)
        {
            var token = HttpContext.Request.Headers.FirstOrDefault(h => h.Key == "token").Value.FirstElementOrDefault() ?? "";
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

                    var response = await TestsStorage.DeleteTestAsync(request.IsById 
                        ? new TestsStorageService.API.DeleteTestRequest(request.TestId) 
                        : new TestsStorageService.API.DeleteTestRequest(request.TestNameFilter));

                    return Ok(new API.DeleteTestResponse());
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        async Task<TestCase> getAuthorNameAsync(string testName, bool returnNotSaved)
        {
            var lstReq = new ListTestsDataRequest(new string[] { testName }, new IntInterval(0, 1), false, returnNotSaved);
            ListTestsDataResponse lstResp = await TestsStorage.ListTestsDataAsync(lstReq);

            return lstResp.Tests.FirstElementOrDefault();
        }
        async Task<TestCase> getAuthorNameAsync(int testId, bool returnNotSaved)
        {
            var lstReq = new ListTestsDataRequest(new int[] { testId }, new IntInterval(0, 1), false, returnNotSaved);
            ListTestsDataResponse lstResp = await TestsStorage.ListTestsDataAsync(lstReq);

            return lstResp.Tests.FirstElement();
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.Route(nameof(IPresentationService.GetTestsAddStateAsync))]
        public async Task<GetTestsAddStateResponse> GetTestsAddState(GetTestsAddStateRequest request)
        {
            var token = HttpContext.Request.Headers.FirstOrDefault(h => h.Key == "token").Value.FirstElementOrDefault() ?? "";
            var userInfResp = await UserService.GetUserInfoAsync(new GGetUserInfoRequest() { Token = token });

            var statusResponse = await StateService.GetTestsAddStateAsync(new StateService.API.GGetTestsAddStateRequest()
            {
                UserName = userInfResp.UserName
            });

            return new GetTestsAddStateResponse(statusResponse.HasBegan);
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.Route(nameof(IPresentationService.SaveRecordedTestAsync))]
        public async Task<IActionResult> SaveRecordedTest(SaveRecordedTestRequest request)
        {
            var token = HttpContext.Request.Headers.FirstOrDefault(h => h.Key == "token").Value.FirstElementOrDefault() ?? "";
            var result = await UserService.ValidateTokenAsync(new GValidateTokenRequest() { Token = token });
            if (result.Valid)
            {
                var userInfResp = await UserService.GetUserInfoAsync(new GGetUserInfoRequest() { Token = token });

                var saveRequest = new SaveTestRequest(request.TestId, request.TestName, request.TestDescription, userInfResp.UserName);
                var saveResponse = await TestsStorage.SaveTestAsync(saveRequest);

                return Ok(new SaveRecordedTestResponse());
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
