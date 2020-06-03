using Google.Protobuf;
using Grpc.Core;
using MessageHub;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PresentationService.API;
using Protobuf;
using RunnerService.API;
using RunnerService.API.Models;
using RunnerService.Db;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TestsStorageService.API;
using Utilities.Extensions;
using Utilities.Types;
using GetTestDetailsRequest = RunnerService.API.Models.GetTestDetailsRequest;
using GetTestDetailsResponse = RunnerService.API.Models.GetTestDetailsResponse;

namespace RunnerService
{
    [ApiController, Microsoft.AspNetCore.Mvc.Route("api/v1")]
    public class MainController : ControllerBase
    {
        [Inject] public RunnerContext Db { get; set; }
        [Inject] public IMessageProducer MessageProducer { get; set; }
        [Inject] public ITestsStorageService TestsStorage { get; set; }
        [Inject] public JsonSerializerSettings JsonSettings { get; set; }

        public MainController(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.Route(nameof(IRunnerService.RunTestAsync))]
        public async Task<RunTestResponse> RunTest(RunTestRequest request)
        {
            var listRequest = new ListTestsDataRequest(request.TestsIdsFilter, new Vectors.IntInterval(0, 1000), true, false);
            ListTestsDataResponse listResponse = await TestsStorage.ListTestsDataAsync(listRequest);

            var testIdFilter = request.TestsIdsFilter.Single();
            var tests = Db.TestRuns
                .IncludeGroup(EntityGroups.ALL, Db)
                .Where(r => r.TestName.StartsWith(testIdFilter))
                .AsEnumerable()
                .Where(r => r.TestName == testIdFilter || r.TestName[testIdFilter.Length] == '.')
                .ToArray();
            foreach (var testFromStorge in listResponse.Tests)
            {
                var exists = tests.Any(r => r.TestName == testFromStorge.TestName);
                if (!exists)
                {
                    tests.Add(new RunnerService.Db.TestRunInfo()
                    {
                        Results = new List<Result>(),
                        RunPlan = new ManualRunPlan(),
                        State = new JustCreatedState(),
                        TestName = testFromStorge.TestName
                    });

                    await Db.TestRuns.AddAsync(tests.LastElement());
                }
            }

            var messages = new List<Func<BeginTestMessage>>();
            foreach (var test in tests)
            {
                if (test.State.State.IsOneOf(State.JustCreated, State.AwaitingStart, State.Ready))
                {
                    var result = new Result()
                    {
                        ResultBase = new PendingCompletionResult()
                        {
                            StartTime = DateTime.UtcNow,
                            StartedByUser = request.UserName,
                            TestId = test.TestId,
                            TestName = test.TestName,
                        }
                    };
                    test.Results.Add(result);
                    test.State = new RunningState();

                    var data = listResponse.Tests.First(t => t.TestName == test.TestName).Data;
                    messages.Add(() => new BeginTestMessage(test.TestId, result.Id, data.Type, data.Data));
                }
            }

            await Db.SaveChangesAsync();

            foreach (var message in messages)
            {
                MessageProducer.FireBeginTest(message());
            }

            return new RunTestResponse();
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.Route(nameof(IRunnerService.GetTestsInfoAsync))]
        public async Task<GetTestsInfoResponse> GetTestsInfo(GetTestsInfoRequest request)
        {
            var infos = await ensureDbPopulated(request.TestsIds.ToArray());

            return new GetTestsInfoResponse(
                infos
                    .Select(i => new API.Models.TestRunInfo(
                        i.TestId,
                        i.TestName,
                        i.Results
                            .OrderByDescending(r => r.ResultBase.StartTime)
                            .FirstOrDefault(r => r.ResultBase?.Result != RunResult.PendingCompletion)
                            ?.ResultBase,
                        i.State,
                        i.RunPlan))
                    .ToArray());
        }

        async Task<Db.TestRunInfo[]> ensureDbPopulated(string[] testIds)
        {
            var infos = await Db.TestRuns
                .IncludeGroup(EntityGroups.ALL, Db)
                .Where(r => testIds.Contains(r.TestName))
                .ToArrayAsync();
            var missingIds = testIds
                .Where(id => infos.NotContains(i => i.TestName == id))
                .ToArray();
            if (missingIds.Length > 0)
            {
                var lstReq = new ListTestsDataRequest(missingIds, new Vectors.IntInterval(0, missingIds.Length), false, false);
                ListTestsDataResponse lstResp = await TestsStorage.ListTestsDataAsync(lstReq);

                var missingRunInfos = lstResp.Tests
                    .Select(ti => new Db.TestRunInfo()
                    {
                        TestId = ti.TestId,
                        TestName = ti.TestName,
                        State = new JustCreatedState(),
                        RunPlan = new ManualRunPlan(),
                    }).ToArray();
                if (missingRunInfos.Length > 0)
                {
                    await Db.TestRuns.AddRangeAsync(missingRunInfos);
                    await Db.SaveChangesAsync();
                    
                    infos = infos.Concat(missingRunInfos).ToArray();
                }
            }

            return infos;
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.Route(nameof(IRunnerService.GetTestDetailsAsync))]
        public async Task<GetTestDetailsResponse> GetTestDetails(GetTestDetailsRequest request)
        {
            var testIdFilter = request.TestIdFilters.Single();
            var dbResults = await Db.TestRuns
                .IncludeGroup(EntityGroups.RESULTS, Db)
                .Where(r => r.TestName.StartsWith(testIdFilter))
                .SelectMany(r => r.Results)
                .OrderByDescending(r => r.ResultBase.StartTime)
                .ToArrayAsync();
            var results = dbResults
                .Select(r => r.ResultBase)
                .Take(request.CountFromEnd)
                .ToArray();

            return new GetTestDetailsResponse(results, dbResults.Length);
        }
    }
}
