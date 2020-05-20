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

        public async Task InitializeAsync()
        {
            Connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5011/SignalRHub")
                .Build();
            await Connection.StartAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await Connection.DisposeAsync();
        }
    }
}
