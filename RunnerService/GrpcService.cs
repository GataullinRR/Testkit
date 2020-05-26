using Google.Protobuf;
using Grpc.Core;
using MessageHub;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PresentationService.API;
using Protobuf;
using RunnerService.API;
using RunnerService.APIModels;
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

namespace RunnerService
{
    [GrpcService]
    public class GrpcService : API.RunnerService.RunnerServiceBase
    {
        [Inject] public RunnerContext Db { get; set; }
        [Inject] public IMessageProducer MessageProducer { get; set; }
        [Inject] public TestsStorageService.API.TestsStorageService.TestsStorageServiceClient TestsStorageService { get; set; }
        [Inject] public TestsSourceService.API.TestsSourceService.TestsSourceServiceClient TestsSourceService { get; set; }
        [Inject] public JsonSerializerSettings JsonSettings { get; set; }

        public GrpcService(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        public override async Task<GRunTestResponse> RunTest(GRunTestRequest gRequest, ServerCallContext context)
        {
            RunTestRequest request = gRequest;
            var listRequest = new ListTestsDataRequest(request.TestsIdsFilter, new Vectors.IntInterval(0, 1000), true);
            ListTestsDataResponse listResponse = await TestsStorageService.ListTestsDataAsync(listRequest);

            var testIdFilter = request.TestsIdsFilter.Single();
            var tests = Db.TestRuns
                .IncludeGroup(EntityGroups.ALL, Db)
                .Where(r => r.TestId.StartsWith(testIdFilter))
                .AsEnumerable()
                .Where(r => r.TestId == testIdFilter || r.TestId[testIdFilter.Length] == '.')
                .ToArray();
            foreach (var testFromStorge in listResponse.Tests)
            {
                var exists = tests.Any(r => r.TestId == testFromStorge.TestId);
                if (!exists)
                {
                    tests.Add(new RunnerService.Db.TestRunInfo()
                    {
                        Results = new List<Result>(),
                        RunPlan = new ManualRunPlan(),
                        State = new JustCreatedState(),
                        TestId = testFromStorge.TestId
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
                        }
                    };
                    test.Results.Add(result);
                    test.State = new RunningState();

                    var data = listResponse.Tests.First(t => t.TestId == test.TestId).Data;
                    messages.Add(() => new BeginTestMessage(test.TestId, result.Id, data.Type, data.Data));
                }
            }

            await Db.SaveChangesAsync();

            foreach (var message in messages)
            {
                MessageProducer.FireBeginTest(message());
            }

            return new RunTestResponse(Protobuf.StatusCode.Ok);
        }

        public override async Task<GGetTestsInfoResponse> GetTestsInfo(GGetTestsInfoRequest request, ServerCallContext context)
        {
            var infos = await ensureDbPopulated(request.TestsIds.ToArray());

            return new GetTestsInfoResponse(
                infos
                    .Select(i => new API.TestRunInfo(
                        i.TestId,
                        i.Results
                            .OrderByDescending(r => r.ResultBase.StartTime)
                            .FirstOrDefault(r => r.ResultBase?.Result != RunResult.PendingCompletion)
                            ?.ResultBase,
                        i.State,
                        i.RunPlan))
                    .ToArray(),
                Protobuf.StatusCode.Ok);
        }

        async Task<Db.TestRunInfo[]> ensureDbPopulated(string[] testIds)
        {
            var infos = await Db.TestRuns
                .IncludeGroup(EntityGroups.ALL, Db)
                .Where(r => testIds.Contains(r.TestId))
                .ToArrayAsync();
            var missingIds = testIds
                .Where(id => infos.NotContains(i => i.TestId == id))
                .ToArray();
            if (missingIds.Length > 0)
            {
                var lstReq = new ListTestsDataRequest(missingIds, new Vectors.IntInterval(0, missingIds.Length), false);
                ListTestsDataResponse lstResp = await TestsStorageService.ListTestsDataAsync(lstReq);

                var missingRunInfos = lstResp.Tests
                    .Select(ti => new Db.TestRunInfo()
                    {
                        TestId = ti.TestId,
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

        public override async Task<GGetTestDetailsResponse> GetTestDetails(GGetTestDetailsRequest gRequest, ServerCallContext context)
        {
            GetTestDetailsRequest request = gRequest;

            var testIdFilter = request.TestIdFilters.Single();
            var dbResults = await Db.TestRuns
                .IncludeGroup(EntityGroups.RESULTS, Db)
                .Where(r => r.TestId.StartsWith(testIdFilter))
                .SelectMany(r => r.Results)
                .OrderByDescending(r => r.ResultBase.StartTime)
                .ToArrayAsync();
            var results = dbResults
                .Select(r => r.ResultBase)
                .Take(request.CountFromEnd)
                .ToArray();

            return new GetTestDetailsResponse(results, dbResults.Length, Protobuf.StatusCode.Ok);
        }
    }
}
