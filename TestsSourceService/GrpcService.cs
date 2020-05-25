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

        public override async Task<GBeginRecordingResponse> BeginRecording(GBeginRecordingRequest request, ServerCallContext context)
        {
            var response = new GBeginRecordingResponse()
            {
                Status = new Protobuf.GResponseStatus()
            };

            var testCase = await CaseSource.GetCaseAsync(request.Filter, default);
            if (testCase == null)
            {
                response.Status.Code = Protobuf.StatusCode.NotFound;

                MessageProducer.FireTestAcquired(new TestAcquiringResultMessage()
                {
                    ResultCode = AcquiringResult.TargetNotFound
                });
            }
            else
            {
                MessageProducer.FireTestAcquired(new TestAcquiringResultMessage()
                {
                    TestId = request.TestId,
                    TestData = testCase.Data,
                    TestType = testCase.TargetType,
                });
            }

            return response;
        }

        public override async Task<GBeginTestResponse> BeginTest(GBeginTestRequest request, ServerCallContext context)
        {
            var response = new GBeginTestResponse()
            {
                Status = new Protobuf.GResponseStatus()
            };

            if (request.TestId == "ER1")
            {
                response.Status.Code = Protobuf.StatusCode.Error;
            }
            else
            {
                var completedMessage = new TestCompletedOnSourceMessage(
                        request.TestId,
                        request.ResultId,
                        new PassedResult()
                        {
                            StartTime = DateTime.UtcNow,
                            Duration = TimeSpan.FromSeconds(Global.Random.NextDouble(0, 10)),
                        });
                MessageProducer.FireTestCompletedOnSource(completedMessage);
            }

            return response;
        }
    }
}
