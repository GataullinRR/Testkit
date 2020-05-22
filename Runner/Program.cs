using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Grpc.Net.Client;
using Grpc.Core;
using Grpc.Net.Client.Web;
using Microsoft.JSInterop;
using System.Reflection;
using Utilities.Extensions;
using Shared;
using Utilities.Types;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using PresentationService.API;

namespace Runner
{
    //[Service(ServiceLifetime.Singleton)]
    //public class AAA : IInitializibleService
    //{
    //    public AAA( IDependencyResolver di)
    //    {
    //        di.ResolveProperties(this);
    //    }

    //    public event Func<TestRecordedWebMessage, Task> R = m => Task.CompletedTask;

    //    [Inject] public IMessageService Messages { get; set; }

    //    public async Task InitializeAsync()
    //    {
    //        Messages.AddMessage("Setup");

    //        var hubConnection = new HubConnectionBuilder()
    //            .WithUrl("https://localhost:5011/signalRHub")
    //            .Build();

    //        hubConnection.On<TestRecordedWebMessage>("TestRecorded", (m) =>
    //        {
    //            Messages.AddMessage($"Test \"{m.DisplayName}\" has been recorded!2");
    //        });

    //        await hubConnection.StartAsync();

    //        Messages.AddMessage(hubConnection.State.ToString());
    //    }
    //}

    public interface IAppInitializationAwaiter
    {
        Task AwaitInitializedAsync();
    }

    [Service(ServiceLifetime.Singleton, RegisterAsPolicy.SelfAndFirstLevelInterfaces)]
    public class ServiceInitializator : IAppInitializationAwaiter
    {
        Task _initializationTask;
        [Inject] public SingletonInitializationService InitializationService { get; set; }

        public ServiceInitializator(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        public async Task BeginInitializationAsync(IServiceProvider serviceProvider)
        {
            _initializationTask = InitializationService.InitializeAsync(serviceProvider);

            await AwaitInitializedAsync();
        }

        public Task AwaitInitializedAsync()
        {
            if (_initializationTask == null)
            {
                throw new InvalidOperationException("Initialization process wasn't started!");
            }
            else
            {
                return _initializationTask;
            }
        }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            var services = builder.Services;
            services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddSingleton(sp => createUserServiceClient(sp.GetRequiredService<ICookieStorage>()));
            services.AddSingleton(sp => createPresentationServiceClient(sp.GetRequiredService<ICookieStorage>()));

            services.AddUINecessaryFeatures();

            await builder.Build().RunAsync();
        }

        static PresentationService.API2.PresentationService.PresentationServiceClient createPresentationServiceClient(ICookieStorage cookies)
        {
            var token = cookies.GetValueAsync(Constants.AUTH_TOKEN_COOKIE).Result;
            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                if (token.IsNotNullOrEmpty())
                {
                    metadata.Add("Authorization", $"Bearer {token}");
                }
                return Task.CompletedTask;
            });

            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
            var channel = GrpcChannel.ForAddress("https://localhost:5011/", new GrpcChannelOptions
            {
                HttpClient = new HttpClient(handler),
                Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
            });
            return new PresentationService.API2.PresentationService.PresentationServiceClient(channel);
        }

        static UserService.API.UserService.UserServiceClient createUserServiceClient(ICookieStorage cookies)
        {
            var token = cookies.GetValueAsync(Constants.AUTH_TOKEN_COOKIE).Result;
            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                if (token.IsNotNullOrEmpty())
                {
                    metadata.Add("Authorization", $"Bearer {token}");
                }
                return Task.CompletedTask;
            });

            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
            var channel = GrpcChannel.ForAddress("https://localhost:5001/", new GrpcChannelOptions
            {
                HttpClient = new HttpClient(handler),
                Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
            });
            return new UserService.API.UserService.UserServiceClient(channel);
        }
    }
}
