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
using Utilities.Types;
using System.Reflection;
using Utilities.Extensions;
using Microsoft.AspNetCore.Components;
using Shared;

namespace Runner
{
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
