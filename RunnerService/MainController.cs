using Google.Protobuf;
using Grpc.Core;
using MessageHub;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PresentationService.API;
using RunnerService.API;
using RunnerService.API.Models;
using RunnerService.Db;
using SharedT;
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
            var listRequest = ListTestsDataRequest.ByNameFilter(request.TestNameFilters, new Vectors.IntInterval(0, 1000), false, true);
            ListTestsDataResponse listResponse = await TestsStorage.ListTestsDataAsync(listRequest);

            var testIdFilter = request.TestNameFilters.Single();
            var tests = Db.TestRuns
                .IncludeGroup(RunnerService.Db.EntityGroups.ALL, Db)
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
                        StartedByUser = request.UserName,
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
            var testIdFilter = request.TestIdFilters.Single();
            var dbResults = await Db.TestRuns
                .IncludeGroup(RunnerService.Db.EntityGroups.RESULTS, Db)
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
