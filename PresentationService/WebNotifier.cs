using DDD;
using MessageHub;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using PresentationService.API;
using System.Threading.Tasks;
using TestsStorageService.API;
using Utilities.Types;
using Vectors;
using Utilities.Extensions;

namespace PresentationService
{
    [Service(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton, RegisterAsPolicy.Self)]
    public class WebNotifier 
    {
        [Inject] public JsonSerializerSettings JsonSettings { get; set; }
        [Inject] public TestsStorageService.API.TestsStorageService.TestsStorageServiceClient TestsStorageService { get; set; }
        [Inject] public IMessageConsumer MessageConsumer { get; set; }
        [Inject] public IHubContext<SignalRHub, IMainHub> Hub { get; set; }

        public WebNotifier(IDependencyResolver di)
        {
            di.ResolveProperties(this);

            MessageConsumer.TestAddedAsync += MessageConsumer_TestRecordedAsync;
            MessageConsumer.TestCompletedAsync += MessageConsumer_TestCompletedAsync;
        }

        async Task MessageConsumer_TestCompletedAsync(TestCompletedMessage arg)
        {
            await Hub.Clients
                .Group(arg.Result.StartedByUser)
                .TestCompleted(new TestCompletedWebMessage()
                { 
                    TestName = arg.TestId, 
                    RunResult = arg.Result
                });
        }

        async Task MessageConsumer_TestRecordedAsync(TestAddedMessage arg)
        {
            await Hub.Clients
                .Group(arg.AuthorName)
                .TestRecorded(new TestAddedWebMessage(arg.TestId, arg.TestName, arg.AuthorName));
        }

        async Task<TestCase> getAuthorNameAsync(string testId)
        {
            var lstReq = new ListTestsDataRequest(new string[] { testId }, new IntInterval(0, 1), false, false);
            ListTestsDataResponse lstResp = await TestsStorageService.ListTestsDataAsync(lstReq);

            return lstResp.Tests.FirstElement();
        }
    }
}
