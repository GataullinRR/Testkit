using Google.Protobuf;
using Grpc.Core;
using Grpc.Core.Logging;
using MessageHub;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PresentationService.API;
using RunnerService.API;
using RunnerService.API.Models;
using RunnerService.Db;
using SharedT;
using SharedT.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TestsStorageService.API;
using Utilities.Extensions;
using Utilities.Interfaces;
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
        [Inject] public ILogger<MainController> Logger { get; set; }

        public MainController(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.Route(nameof(IRunnerService.RunTestAsync))]
        public async Task<RunTestResponse> RunTest(RunTestRequest request)
        {
            var listRequest = new ListTestsDataRequest(request.FilteringOrders, new Vectors.IntInterval(0, 1000), true, false);
            var listResponse = await TestsStorage.ListTestsDataAsync(listRequest);
           
            var testsQuery = Db.TestRuns
                .IncludeGroup(RunnerService.Db.EntityGroups.ALL, Db)
                .Filter(Logger, request.FilteringOrders);
            var tests = await testsQuery.ToArrayAsync();
            foreach (var testFromStorge in listResponse.Tests)
            {
                var exists = tests.Any(r => r.TestName == testFromStorge.TestName);
                if (!exists)
                {
                    tests.Add(new RunnerService.Db.TestRunInfo()
                    {
                        TestId = testFromStorge.TestId,
                        Results = new List<Result>(),
                        RunPlan = new ManualRunPlan(),
                        TestName = testFromStorge.TestName,
                    });

                    await Db.TestRuns.AddAsync(tests.LastElement());
                }
            }

            var messages = new List<Func<BeginTestMessage>>();
            foreach (var test in tests)
            {
                var result = new Result()
                {
                    ResultBase = new PendingCompletionResult()
                    {
                        StartTime = DateTime.UtcNow,
                        StartedByUser = request.StartedByUser,
                        TestId = test.TestId,
                        TestName = test.TestName,
                        State = new StateInfo("", null, false)
                    }
                };
                test.Results.Add(result);

                var data = listResponse.Tests.First(t => t.TestName == test.TestName).Data;
                messages.Add(() => new BeginTestMessage(test.TestId, result.Id, data.Type, data.Data));
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
            var infos = await ensureDbPopulated(request.TestIds);

            return new GetTestsInfoResponse(ges().ToArray());

            IEnumerable<API.Models.TestRunInfo> ges()
            {
                foreach (var i in infos)
                {
                    var lastResult = i.Results
                        .OrderByDescending(r => r.ResultBase.StartTime)
                        .FirstOrDefault()
                        ?.ResultBase;

                    StateBase state = new JustCreatedState();
                    if (i.Results.Any(r => r.ResultBase.Result == RunResult.Running))
                    {
                        state = new RunningState();
                    }
                    else
                    {
                        state = lastResult?.Result switch
                        {
                            RunResult.Aborted => new ReadyState(),
                            RunResult.Passed => new ReadyState(),
                            RunResult.Error => new ReadyState(),
                            RunResult.FatalError => new ReadyState(),
                            RunResult.Running => new RunningState(),
                            null => new JustCreatedState(),

                            _ => throw new NotSupportedException()
                        };
                    }

                    yield return new API.Models.TestRunInfo(
                        i.TestId,
                        i.TestName,
                        lastResult,
                        state,
                        i.RunPlan);
                }
            }
        }

        async Task<Db.TestRunInfo[]> ensureDbPopulated(int[] testIds)
        {
            var infos = await Db.TestRuns
                .IncludeGroup(RunnerService.Db.EntityGroups.ALL, Db)
                .Where(r => testIds.Contains(r.TestId))
                .ToArrayAsync();
            var missingIds = testIds
                .Where(id => infos.NotContains(i => i.TestId == id))
                .ToArray();
            if (missingIds.Length > 0)
            {
                var lstReq = ListTestsDataRequest.ByIds(missingIds, new Vectors.IntInterval(0, missingIds.Length), false, false);
                ListTestsDataResponse lstResp = await TestsStorage.ListTestsDataAsync(lstReq);

                var missingRunInfos = lstResp.Tests
                    .Select(ti => new Db.TestRunInfo()
                    {
                        TestId = ti.TestId,
                        TestName = ti.TestName,
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
            var ff = Db.TestRuns
                .IncludeGroup(RunnerService.Db.EntityGroups.RESULTS, Db)
                .Filter(Logger, request.FilteringOrders);
            var dbResults = await ff
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
