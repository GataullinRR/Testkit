using MessageHub;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using PresentationService.API;
using System.Threading.Tasks;
using Utilities.Types;

namespace PresentationService
{
    [Service(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
    public class RecordedNotifier 
    {
        [Inject] public IMessageConsumer MessageConsumer { get; set; }
        [Inject] public IHubContext<SignalRHub, IMainHub> Hub { get; set; }

        public RecordedNotifier(IDependencyResolver di)
        {
            di.ResolveProperties(this);

            MessageConsumer.TestRecordedAsync += MessageConsumer_TestRecordedAsync;
        }

        async Task MessageConsumer_TestRecordedAsync(TestRecordedMessage arg)
        {
            await Hub.Clients.All.TestRecorded("AA", new TestRecordedWebMessage() { DisplayName = "Isn't yet supported" });
        }
    }
}
