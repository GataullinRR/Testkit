using System.Threading.Tasks;
using Utilities.Types;
using Utilities.Extensions;
using TestsSourceService.API;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Google.Protobuf;
using MessageHub;
using Shared;
using RunnerService.APIModels;
using System;
using Utilities;

namespace ExampleTestsSourceService
{
    [GrpcService]
    public class GrpcService : TestsSourceService.API.TestsSourceService.TestsSourceServiceBase
    {
        [Inject] ICaseSource CaseSource { get; set; }
        [Inject] IMessageProducer MessageProducer { get; set; }

        public GrpcService(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        public override async Task<BeginRecordingResponse> BeginRecording(BeginRecordingRequest request, ServerCallContext context)
        {
            var response = new BeginRecordingResponse()
            {
                Status = new Protobuf.ResponseStatus()
            };

            var testCase = await CaseSource.GetCaseAsync(request.Filter, default);
            if (testCase == null)
            {
                response.Status.Code = Protobuf.StatusCode.NotFound;
            }
            else
            {
                MessageProducer.FireTestAcquired(new TestAcquiredMessage() 
                {
                    OperationContext = new MessageHub.OperationContext()
                    {
                         OperationId = request.OperationContext.OperationId,
                         UserName = request.OperationContext.UserName,
                    },
                    Test = testCase
                });
            }

            return response;
        }

        public override async Task<BeginTestResponse> BeginTest(BeginTestRequest request, ServerCallContext context)
        {
            var response = new BeginTestResponse()
            {
                Status = new Protobuf.ResponseStatus()
            };

            if (request.TestSourceId == "ER1")
            {
                response.Status.Code = Protobuf.StatusCode.Error;
            }
            else
            {
                MessageProducer.FireTestCompletedOnSource(new TestCompletedOnSourceMessage()
                {
                    OperationContext = new MessageHub.OperationContext()
                    {
                        OperationId = request.OperationContext.OperationId,
                        UserName = request.OperationContext.UserName,
                    },
                    TestSourceId = request.TestSourceId,
                    Result = new PassedResult()
                    {
                        StartTime = DateTime.UtcNow,
                        Duration = TimeSpan.FromSeconds(Global.Random.NextDouble(0, 10)),
                    }
                });
            }

            return response;
        }
    }
}
