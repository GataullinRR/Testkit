using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Utilities.Types;

namespace PresentationService.API
{
    [Service(ServiceLifetime.Singleton)]
    class WebMessageHubConnection : IWebMessageHubConnectionProvider, IInitializibleService, IAsyncDisposable
    {
        public HubConnection Connection { get; private set; }

        public WebMessageHubConnection()
        {
            Connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5011/signalRHub")
                .Build();
        }

        public async Task InitializeAsync()
        {
            await Connection.StartAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await Connection.DisposeAsync();
        }
    }
}
