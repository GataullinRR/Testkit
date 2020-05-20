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

namespace ExampleTestsSourceService
{
    [GrpcService]
    public class API : TestsSourceService.API.TestsSourceService.TestsSourceServiceBase
    {
        [Inject] ICaseSource CaseSource { get; set; }
        [Inject] IMessageProducer MessageProducer { get; set; }

        public API(IDependencyResolver di)
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
                MessageProducer.FireTestAcquired(new TestAcquiredMessage() { Test = testCase });
            }

            return response;
        }
    }
}
