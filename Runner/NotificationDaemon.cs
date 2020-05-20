using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Types;
using Microsoft.AspNetCore.Components;
using PresentationService.API;

namespace Runner
{
    [Service(ServiceLifetime.Singleton)]
    class NotificationDaemon
    {
        [Inject] public IWebMessageHub WebMessageHub { get; set; }
        [Inject] public IMessageService MessageService { get; set; }

        public NotificationDaemon(IDependencyResolver di)
        {
            di.ResolveProperties(this);

            WebMessageHub.TestRecordedAsync += WebMessageHub_TestRecordedAsync;
        }

        async Task WebMessageHub_TestRecordedAsync(TestRecordedWebMessage arg)
        {
            MessageService.AddMessage($"\"{arg.DisplayName}\" has been added");
        }
    }
}
