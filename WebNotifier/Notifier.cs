using MessageHub;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TestsStorageService.API;
using Utilities.Types;
using Vectors;
using Utilities.Extensions;
using WebNotificationService.API;
using System.Threading;

namespace WebNotificationService
{
    [Service(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton, RegisterAsPolicy.Self)]
    public class Notifier : Microsoft.Extensions.Hosting.IHostedService
    {
        [Inject] public JsonSerializerSettings JsonSettings { get; set; }
        [Inject] public ITestsStorageService TestsStorage { get; set; }
        [Inject] public IMessageConsumer MessageConsumer { get; set; }
        [Inject] public IHubContext<SignalRHub, IMainHub> Hub { get; set; }

        public Notifier(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            MessageConsumer.TestAddedAsync += MessageConsumer_TestAddedAsync;
            MessageConsumer.TestCompletedAsync += MessageConsumer_TestCompletedAsync;
            MessageConsumer.TestDeletedAsync += MessageConsumer_TestDeletedAsync;
            MessageConsumer.BeginTestAsync += MessageConsumer_BeginTestAsync;
            MessageConsumer.TestRecordedAsync += MessageConsumer_TestRecordedAsync;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

        }

        async Task MessageConsumer_TestRecordedAsync(TestRecordedMessage arg)
        {
            await Hub.Clients.All.TestRecorded(new TestRecordedWebMessage(arg.TestId));
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
    }
}
