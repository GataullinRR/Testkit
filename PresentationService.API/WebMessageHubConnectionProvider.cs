using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Extensions;
using Utilities.Types;

namespace PresentationService.API
{
    [Service(ServiceLifetime.Singleton)]
    class WebMessageHubConnectionProvider : IWebMessageHubConnectionProvider, IInitializibleService, IAsyncDisposable
    {
        readonly SemaphoreSlim _locker = new SemaphoreSlim(1);

        public HubConnection Connection { get; private set; }

        public WebMessageHubConnectionProvider(IDependencyResolver di)
        {
            di.ResolveProperties(this);

            Connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5011/signalRHub")
                .AddNewtonsoftJsonProtocol(options =>
                {
                    options.PayloadSerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
                })
                .Build();
        }

        public async Task BindUserAsync(string token)
        {
            using (await _locker.AcquireAsync())
            {
                await Connection.SendAsync("Bind", token);
            }
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
