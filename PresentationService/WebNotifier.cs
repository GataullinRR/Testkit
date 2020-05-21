using MessageHub;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using PresentationService.API;
using System.Threading.Tasks;
using Utilities.Types;

namespace PresentationService
{
    [Service(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
    public class WebNotifier 
    {
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
            await Hub.Clients.All.TestCompleted(new TestCompletedWebMessage()
            { 
                TestId = arg.TestId, 
                RunResult = arg.Result 
            });
        }

        async Task MessageConsumer_TestRecordedAsync(TestRecordedMessage arg)
        {
            await Hub.Clients.All.TestRecorded(new TestRecordedWebMessage() { DisplayName = "Isn't yet supported" });
        }
    }
}
