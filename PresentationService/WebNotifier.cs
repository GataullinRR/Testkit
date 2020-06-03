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
        [Inject] public ITestsStorageService TestsStorage { get; set; }
        [Inject] public IMessageConsumer MessageConsumer { get; set; }
        [Inject] public IHubContext<SignalRHub, IMainHub> Hub { get; set; }

        public WebNotifier(IDependencyResolver di)
        {
            di.ResolveProperties(this);

            MessageConsumer.TestAddedAsync += MessageConsumer_TestAddedAsync;
            MessageConsumer.TestCompletedAsync += MessageConsumer_TestCompletedAsync;
            MessageConsumer.TestDeletedAsync += MessageConsumer_TestDeletedAsync;
            MessageConsumer.BeginTestAsync += MessageConsumer_BeginTestAsync;
            MessageConsumer.TestRecordedAsync += MessageConsumer_TestRecordedAsync;
            MessageConsumer.TestAddProgressChangedAsync += MessageConsumer_TestAddProgressChangedAsync;
        }

        async Task MessageConsumer_TestAddProgressChangedAsync(TestAddProgressChangedMessage arg)
        {
            await Hub.Clients
                .Group(arg.UserName)
                .TestAddProgressChanged(new TestAddProgressChangedWebMessage(arg.UserName, arg.Log));
        }

        async Task MessageConsumer_TestRecordedAsync(TestRecordedMessage arg)
        {
            await Hub.Clients
                .Group(arg.AuthorName)
                .TestRecorded(new TestRecordedWebMessage(arg.TestId, arg.AuthorName));
        }

        async Task MessageConsumer_TestAddedAsync(TestAddedMessage arg)
        {
            await Hub.Clients.All.TestAdded(new TestAddedWebMessage(arg.TestId, arg.TestName, arg.AuthorName));
        }

        async Task MessageConsumer_BeginTestAsync(BeginTestMessage arg)
        {
            await Hub.Clients.All.TestBegun(new TestBegunWebMessage(arg.TestId));
        }

        async Task MessageConsumer_TestDeletedAsync(TestDeletedMessage arg)
        {
            await Hub.Clients.All.TestDeleted(new TestDeletedWebMessage(arg.TestId));
        }

        async Task MessageConsumer_TestCompletedAsync(TestCompletedMessage arg)
        {
            await Hub.Clients.All.TestCompleted(new TestCompletedWebMessage(arg.TestId, arg.Result));
        }

        async Task<TestCase> getAuthorNameAsync(string testId)
        {
            var lstReq = new ListTestsDataRequest(new string[] { testId }, new IntInterval(0, 1), false, false);
            ListTestsDataResponse lstResp = await TestsStorage.ListTestsDataAsync(lstReq);

            return lstResp.Tests.FirstElement();
        }
    }
}
