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
using Microsoft.Extensions.Logging;

namespace WebNotificationService
{
    [Service(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton, RegisterAsPolicy.Self)]
    public class Notifier : Microsoft.Extensions.Hosting.IHostedService
    {
        [Inject] public JsonSerializerSettings JsonSettings { get; set; }
        [Inject] public ITestsStorageService TestsStorage { get; set; }
        [Inject] public IMessageConsumer MessageConsumer { get; set; }
        [Inject] public IHubContext<SignalRHub, IMainHub> Hub { get; set; }
        [Inject] public ILogger<Notifier> Logger { get; set; }

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
            MessageConsumer.TestCancelledAsync += MessageConsumer_TestCancelledAsync;
            MessageConsumer.TestResultStateUpdatedAsync += MessageConsumer_TestResultStateUpdatedAsync;
        }

        async Task MessageConsumer_TestResultStateUpdatedAsync(TestResultStateUpdatedMessage arg)
        {
            Logger.LogTrace("EntryChanged(TestResultChangedWebMessage)");

            await Hub.Clients.All.EntryChanged(new TestResultChangedWebMessage(arg.TestId, arg.ResultId, Change.Modified));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

        }

        async Task MessageConsumer_TestCancelledAsync(TestCancelledMessage arg)
        {
            Logger.LogTrace("TestCancelled");

            await Hub.Clients.All.TestCancelled(new TestCancelledWebMessage(arg.TestId, arg.TestName));
        }

        async Task MessageConsumer_TestRecordedAsync(TestRecordedMessage arg)
        {
            Logger.LogTrace("TestRecorded");

            await Hub.Clients.All.TestRecorded(new TestRecordedWebMessage(arg.TestId));
        }

        async Task MessageConsumer_TestAddedAsync(TestAddedMessage arg)
        {
            Logger.LogTrace("TestAdded");

            await Hub.Clients.All.TestAdded(new TestAddedWebMessage(arg.TestId, arg.TestName, arg.AuthorName));
        }

        async Task MessageConsumer_BeginTestAsync(BeginTestMessage arg)
        {
            Logger.LogTrace("TestBegun");

            await Hub.Clients.All.TestBegun(new TestBegunWebMessage(arg.TestId));
        }

        async Task MessageConsumer_TestDeletedAsync(TestDeletedMessage arg)
        {
            Logger.LogTrace("TestDeletedWebMessage");

            await Hub.Clients.All.TestDeleted(new TestDeletedWebMessage(arg.TestId));
        }

        async Task MessageConsumer_TestCompletedAsync(TestCompletedMessage arg)
        {
            Logger.LogTrace("TestCompleted");

            await Hub.Clients.All.TestCompleted(new TestCompletedWebMessage(arg.TestId, arg.Result));
        }
    }
}
