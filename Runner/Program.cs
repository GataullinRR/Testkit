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
using Microsoft.JSInterop;
using System.Reflection;
using Utilities.Extensions;
using SharedT;
using Microsoft.AspNetCore.SignalR.Client;
using PresentationService.API;
using UserService.API;
using WebNotificationService.API;

namespace Runner
{
    public class Program
    {   
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            var envName = builder.HostEnvironment.Environment;
            var assembly = Assembly.GetExecutingAssembly();
            var mainConfig = assembly.GetManifestResourceStream("appsettings.json");
            var envConfig = assembly.GetManifestResourceStream($"appsettings.{envName}.json");
            var configBuilder = new ConfigurationBuilder()
                .AddJsonStream(mainConfig)
                .AddJsonStream(envConfig);
            var config = configBuilder.Build();

            var services = builder.Services;
            services.AddUINecessaryFeatures();
            services.AddUserService(config.GetSection("Services"));
            services.AddPresentationService(config.GetSection("Services"));
            services.AddWebNotificationService(config.GetSection("Services"));

            await builder.Build().RunAsync();
        }
    }
}
