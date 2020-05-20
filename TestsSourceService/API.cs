using System.Threading.Tasks;
using Utilities.Types;
using Utilities.Extensions;
using TestsSourceService.API;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Google.Protobuf;
using MessageHub;

namespace ExampleTestsSourceService
{
    public class API : TestsSourceService.API.TestsSourceService.TestsSourceServiceBase
    {
        [Inject] ICaseSource CaseSource { get; set; }
        [Inject] IMessageProducer MessageProducer { get; set; }

        public API(IDependencyResolver di)
        {
            di.ResolveProperties(CaseSource);
        }

        public override async Task<BeginRecordingResponse> BeginRecording(BeginRecordingRequest request, ServerCallContext context)
        {
            var testCase = await CaseSource.GetCaseAsync(request.Filter, default);

            MessageProducer.FireTestAcquired(new TestAcquiredMessage() { Test = testCase });

            return new BeginRecordingResponse();
        }
    }
}
