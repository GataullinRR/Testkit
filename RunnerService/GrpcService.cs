using Google.Protobuf;
using Grpc.Core;
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
        [Inject] public TestsStorageService.API.TestsStorageService.TestsStorageServiceClient TestsStorageService { get; set; }
        [Inject] public TestsSourceService.API.TestsSourceService.TestsSourceServiceClient TestsSourceService { get; set; }
        [Inject] public JsonSerializerSettings JsonSettings { get; set; }

        public GrpcService(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        public override async Task<GRunTestResponse> RunTest(GRunTestRequest request, ServerCallContext context)
        {
            var response = new GRunTestResponse()
            {
                Status = new Protobuf.GResponseStatus()
            };

            var listRequest = new TestsStorageService.API.GListTestsDataRequest();
            listRequest.ByIds.Add(request.TestId);
            listRequest.IncludeData = true;
            var listResponse = await TestsStorageService.ListTestsDataAsync(listRequest);
            var caseInfo = JsonConvert.DeserializeObject<global::TestsStorageService.Db.TestCase[]>(listResponse.Tests.ToStringUtf8(), JsonSettings)[0];

            var test = await Db.TestRuns
                .IncludeGroup(EntityGroups.ALL, Db)
                .FirstOrDefaultAsync(r => r.TestId == request.TestId);
            if (test == null)
            {
                test = new Db.TestRunInfo()
                {
                    State = new JustCreatedState(),
                    TestId = request.TestId,
                    RunPlan = new ManualRunPlan(),
                };

                Db.TestRuns.Add(test);
                await Db.SaveChangesAsync();
            }

            var result = new Result()
            {
                ResultBase = new PendingCompletionResult()
                {
                    StartTime = DateTime.UtcNow,
                    StartedByUser = request.UserName
                }
            };
            test.Results.Add(result);

            if (test.State.State.IsOneOf(State.JustCreated, State.AwaitingStart, State.Ready))
            {
                test.State = new RunningState();
                await Db.SaveChangesAsync();

                var beginRequest = new TestsSourceService.API.GBeginTestRequest();
                beginRequest.TestData = ByteString.CopyFrom(caseInfo.Data.Data);
                beginRequest.TestType = caseInfo.Data.Type;
                beginRequest.TestId = request.TestId;
                beginRequest.ResultId = result.Id;
                var beginResponse = await TestsSourceService.BeginTestAsync(beginRequest);
            }
            else
            {
                response.Status.Code = Protobuf.StatusCode.Error;
                response.Status.Description = "Test already running or so...";
            }

            return response;
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
                var lstReq = new GListTestsDataRequest();
                lstReq.ByIds.AddRange(missingIds);
                var lstResp = await TestsStorageService.ListTestsDataAsync(lstReq);
                var tests = JsonConvert.DeserializeObject<TestsStorageService.Db.TestCase[]>(lstResp.Tests.ToStringUtf8(), JsonSettings);

                var missingRunInfos = tests
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

            var run = await Db.TestRuns
                .IncludeGroup(EntityGroups.RESULTS, Db)
                .FirstOrDefaultAsync(r => r.TestId == request.TestId);
            var results = run.Results
                .OrderByDescending(r => r.ResultBase.StartTime)
                .Select(r => r.ResultBase)
                .Take(request.CountFromEnd)
                .ToArray();

            return new GetTestDetailsResponse(results, run.Results.Count, Protobuf.StatusCode.Ok);
        }
    }
}
