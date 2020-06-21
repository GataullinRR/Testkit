using DDD;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PresentationService.API;
using RunnerService.API;
using RunnerService.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestsStorageService.API;
using UserService.API;
using Utilities.Types;
using Utilities.Extensions;
using SharedT.Types;
using Vectors;
using MessageHub;
using Microsoft.AspNetCore.Mvc;

namespace PresentationService
{
    [ApiController, Microsoft.AspNetCore.Mvc.Route("api/v1")]
    public class MainController : ControllerBase
    {
        [Inject] public IUserService UserService { get; set; }
        [Inject] public ITestsStorageService TestsStorage { get; set; }
        [Inject] public IRunnerService RunnerService { get; set; }
        [Inject] public ILogger<MainController> Logger { get; set; }
        [Inject] public JsonSerializerSettings JsonSettings { get; set; }

        public MainController(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.Route(nameof(IPresentationService.ListTestsAsync))]
        public async Task<ListTestsResponse> ListTests(ListTestsRequest request)
        {
            var lstRequest = new ListTestsDataRequest(request.FilteringOrders, request.Range, false, request.ReturnNotSaved);
            var tests = await TestsStorage.ListTestsDataAsync(lstRequest);

            var testsIds = tests.Tests.Select(c => c.TestId).ToArray();
            var getInfosR = new RunnerService.API.Models.GetTestsInfoRequest(testsIds);
            RunnerService.API.Models.GetTestsInfoResponse getInfosResp = request.ReturnNotSaved 
                ? (RunnerService.API.Models.GetTestsInfoResponse)null
                : (RunnerService.API.Models.GetTestsInfoResponse)await RunnerService.GetTestsInfoAsync(getInfosR);

            var fullInfos = tests.Tests.Select(c => (Case: c, RunInfo: getInfosResp == null ? (RunnerService.API.Models.TestRunInfo)null : getInfosResp.RunInfos.FirstOrDefault(r => r.TestId == c.TestId)));
            //var fullInfos = tests.Tests.Zip(getInfosResp?.RunInfos ?? ((RunnerService.API.Models.TestRunInfo)null).Repeat(tests.Tests.Length).ToArray() , (Case, RunInfo) => (Case, RunInfo));

            var response = new ListTestsResponse(ddd().ToArray(), tests.TotalCount);

            return response;

            IEnumerable<TestInfo> ddd()
            {
                foreach (var info in fullInfos)
                {
                    yield return new TestInfo()
                    {
                        TestId = info.Case.TestId,
                        TestName = info.Case.TestName,
                        Author = info.Case.AuthorName == null 
                            ? null 
                            : new GetUserInfoResponse(info.Case.AuthorName, null, null),
                        Target = new TestCaseInfo() 
                        {  
                            DisplayName = info.Case.TestDescription, 
                            TargetType = info.Case?.Data?.Type, 
                            Parameters = info.Case?.Data?.Parameters,
                            CreateDate = info.Case?.CreationDate ?? default,
                            KeyParameters = info.Case?.Data?.KeyParameters.Select(p => p.Key + "###" + p.Value).Aggregate(Environment.NewLine) // Someone, pls, rewrite :D
                        },
                        State = info.RunInfo?.State,
                        LastResult = info.RunInfo?.LastResult,
                        RunPlan = info.RunInfo?.RunPlan,
                        CreationState = info.Case.State
                    };
                }
            }
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.Route(nameof(IPresentationService.BeginTestAsync))]
        public async Task<IActionResult> BeginTest(BeginTestRequest request)
        {
            var token = HttpContext.Request.Headers.FirstOrDefault(h => h.Key == "token").Value.FirstElementOrDefault() ?? "";

            var result = await UserService.ValidateTokenAsync(new ValidateTokenRequest(token));
            if (result.IsValid)
            {
                var userInfResp = await UserService.GetUserInfoAsync(new GetUserInfoRequest(token));

                var runReq = new RunnerService.API.Models.RunTestRequest(userInfResp.UserName, request.FilteringOrders);
                var runResponse = await RunnerService.RunTestAsync(runReq);

                return Ok(new BeginTestResponse());
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.Route(nameof(IPresentationService.GetTestDetailsAsync))]
        public async Task<API.GetTestDetailsResponse> GetTestDetails(API.GetTestDetailsRequest request)
        {
            return await RunnerService.GetTestDetailsAsync(request);
        }

        [HttpDelete, Microsoft.AspNetCore.Mvc.Route(nameof(IPresentationService.DeleteTestAsync))]
        public async Task<IActionResult> DeleteTest(API.DeleteTestRequest request)
        {
            var token = HttpContext.Request.Headers.FirstOrDefault(h => h.Key == "token").Value.FirstElementOrDefault() ?? "";
            var result = await UserService.ValidateTokenAsync(new ValidateTokenRequest(token));
            if (result.IsValid)
            {
                var userInfResp = await UserService.GetUserInfoAsync(new GetUserInfoRequest(token));
                var userName = userInfResp.UserName;
                var testAuthor = (await getAuthorNameAsync(request.FilteringOrders, true)
                    ?? await getAuthorNameAsync(request.FilteringOrders, false))
                    .AuthorName;
                if (userName == testAuthor)
                {
                    var response = await TestsStorage.DeleteTestAsync(new TestsStorageService.API.DeleteTestRequest(request.FilteringOrders));

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

        async Task<TestCase> getAuthorNameAsync(IFilterOrder[] filterOrders, bool returnNotSaved)
        {
            var lstReq = new ListTestsDataRequest(filterOrders, new IntInterval(0, 1), false, returnNotSaved);
            var lstResp = await TestsStorage.ListTestsDataAsync(lstReq);

            return lstResp.Tests.FirstElementOrDefault();
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.Route(nameof(IPresentationService.SaveRecordedTestAsync))]
        public async Task<IActionResult> SaveRecordedTest(SaveRecordedTestRequest request)
        {
            var token = HttpContext.Request.Headers.FirstOrDefault(h => h.Key == "token").Value.FirstElementOrDefault() ?? "";
            var result = await UserService.ValidateTokenAsync(new ValidateTokenRequest(token));
            if (result.IsValid)
            {
                var userInfResp = await UserService.GetUserInfoAsync(new GetUserInfoRequest(token));

                var saveRequest = new SaveTestRequest(request.TestId, request.TestName, request.TestDescription, userInfResp.UserName);
                var saveResponse = await TestsStorage.SaveTestAsync(saveRequest);

                return Ok(new SaveRecordedTestResponse());
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.Route(nameof(IPresentationService.CancelTestAsync))]
        public async Task<IActionResult> CancelTest(CancelTestRequest request, [FromServices]IMessageProducer messageProducer)
        {
            var token = HttpContext.Request.Headers.FirstOrDefault(h => h.Key == "token").Value.FirstElementOrDefault() ?? "";
            var result = await UserService.ValidateTokenAsync(new ValidateTokenRequest(token));
            if (result.IsValid)
            {
                messageProducer.FireCancelTest(new CancelTestMessage(request.FilteringOrders));
                
                return Ok(new CancelTestResponse());
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
