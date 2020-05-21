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
                .FirstOrDefaultAsync(r => r.TestId == request.TestId);
            if (test == null)
            {
                test = new TestRunInfo()
                {
                    State = new AwaitingStartState(),
                    LastRun = null,
                    TestId = request.TestId,
                    TestSourceId = caseInfo.CaseInfo.CaseSourceId,
                };

                Db.TestRuns.Add(test);
                await Db.SaveChangesAsync();
            }

            if (test.State.State.IsOneOf(State.AwaitingStart, State.Ready))
            {
                var beginRequest = new TestsSourceService.API.BeginTestRequest();
                beginRequest.TestData = ByteString.CopyFrom(caseInfo.CaseInfo.Data);
                beginRequest.TestSourceId = caseInfo.CaseInfo.CaseSourceId;
                var beginResponse = await TestsSourceService.BeginTestAsync(beginRequest);

                test.State = new RunningState();
            }
            else
            {
                response.Status.Code = Protobuf.StatusCode.Error;  
            }

            return response;
        }
    }
}
