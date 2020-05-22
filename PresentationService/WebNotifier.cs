using DDD;
using MessageHub;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using PresentationService.API;
using System.Threading.Tasks;
using TestsStorageService.API;
using Utilities.Types;

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

            MessageConsumer.TestRecordedAsync += MessageConsumer_TestRecordedAsync;
            MessageConsumer.TestCompletedAsync += MessageConsumer_TestCompletedAsync;
        }

        async Task MessageConsumer_TestCompletedAsync(TestCompletedMessage arg)
        {
           // var test = await getAuthorNameAsync(arg.TestId);
           
            await Hub.Clients
                .Group(arg.OperationContext.UserName)
                .TestCompleted(new TestCompletedWebMessage()
                { 
                    TestId = arg.TestId, 
                    RunResult = arg.Result
                });
        }

        async Task MessageConsumer_TestRecordedAsync(TestRecordedMessage arg)
        {
            var test = await getAuthorNameAsync(arg.TestId);

            await Hub.Clients
                .Group(test.AuthorName)
                .TestRecorded(new TestRecordedWebMessage() { DisplayName = test.CaseInfo.DisplayName });
        }

        async Task<TestCase> getAuthorNameAsync(string testId)
        {
            var lstReq = new ListTestsDataRequest();
            lstReq.ByIds.Add(testId);
            var lstResp = await TestsStorageService.ListTestsDataAsync(lstReq);

            return JsonConvert.DeserializeObject<TestCase[]>(lstResp.Tests.ToStringUtf8(), JsonSettings)[0];
        }
    }
}
