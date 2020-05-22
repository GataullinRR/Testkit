using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

        public override async Task<RunTestResponse> RunTest(RunTestRequest request, ServerCallContext context)
        {
            var response = new RunTestResponse()
            {
                Status = new Protobuf.ResponseStatus()
            };

            var listRequest = new TestsStorageService.API.ListTestsDataRequest();
            listRequest.ByIds.Add(request.TestId);
            listRequest.IncludeData = true;
            var listResponse = await TestsStorageService.ListTestsDataAsync(listRequest);
            var caseInfo = JsonConvert.DeserializeObject<TestCase[]>(listResponse.Tests.ToStringUtf8(), JsonSettings)[0];

            var test = await Db.TestRuns
                .Include(r => r.State)
                .Include(r => r.LastRun)
                .Include(r => r.RunPlan)
                .FirstOrDefaultAsync(r => r.TestId == request.TestId);
            if (test == null)
            {
                test = new TestRunInfo()
                {
                    State = new JustCreatedState(),
                    LastRun = null,
                    TestId = request.TestId,
                    TestSourceId = caseInfo.CaseInfo.CaseSourceId,
                    RunPlan = new ManualRunPlan()
                };

                Db.TestRuns.Add(test);
                await Db.SaveChangesAsync();
            }

            if (test.State.State.IsOneOf(State.JustCreated, State.AwaitingStart, State.Ready))
            {
                test.State = new RunningState();
                await Db.SaveChangesAsync();

                var beginRequest = new TestsSourceService.API.BeginTestRequest();
                beginRequest.TestData = ByteString.CopyFrom(caseInfo.CaseInfo.Data);
                beginRequest.TestSourceId = caseInfo.CaseInfo.CaseSourceId;
                var beginResponse = await TestsSourceService.BeginTestAsync(beginRequest);
            }

            return response;
        }

        public override async Task<GetTestsInfoResponse> GetTestsInfo(GetTestsInfoRequest request, ServerCallContext context)
        {
            var response = new GetTestsInfoResponse()
            {
                Status = new Protobuf.ResponseStatus()
            };

            var infos = await ensureDbPopulated(request.TestsIds.ToArray());
            response.Infos = ByteString.CopyFromUtf8(JsonConvert.SerializeObject(infos, JsonSettings));

            return response;
        }

        async Task<TestRunInfo[]> ensureDbPopulated(string[] testIds)
        {
            var infos = await Db.TestRuns
                .Include(r => r.LastRun)
                .Include(r => r.State)
                .Include(r => r.RunPlan)
                .Where(r => testIds.Contains(r.TestId))
                .ToArrayAsync();
            var missingIds = testIds
                .Where(id => infos.NotContains(i => i.TestId == id))
                .ToArray();
            if (missingIds.Length > 0)
            {
                var lstReq = new ListTestsDataRequest();
                lstReq.ByIds.AddRange(missingIds);
                var lstResp = await TestsStorageService.ListTestsDataAsync(lstReq);
                var tests = JsonConvert.DeserializeObject<TestCase[]>(lstResp.Tests.ToStringUtf8(), JsonSettings);

                var missingRunInfos = tests
                    .Select(ti => new TestRunInfo()
                    {
                        TestId = ti.Id,
                        TestSourceId = ti.CaseInfo.CaseSourceId,
                        State = new JustCreatedState(),
                        RunPlan = new ManualRunPlan(),
                        LastRun = null,
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
    }
}
