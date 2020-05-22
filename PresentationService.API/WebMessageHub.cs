using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities.Types;
using Utilities.Extensions;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components;

namespace PresentationService.API
{
    [Service(ServiceLifetime.Transient)]
    class WebMessageHub : IWebMessageHub, IDisposable
    {
        readonly IDisposable _subscriptions;

        public event Func<TestRecordedWebMessage, Task> TestRecordedAsync = m => Task.CompletedTask;

        [Inject] public IWebMessageHubConnectionProvider ConnectionProvider {get; set;}

        public WebMessageHub(IDependencyResolver di)
        {
            di.ResolveProperties(this);

            _subscriptions = ConnectionProvider.Connection.On<TestRecordedWebMessage>("TestRecorded", async (message) =>
            {
                await TestRecordedAsync.InvokeAndWaitAsync(message);
            });
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }
    }
}
