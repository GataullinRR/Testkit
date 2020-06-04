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
using Microsoft.AspNetCore.SignalR.Client;
using PresentationService.API;
using UserService.API;

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
            services.AddHttpClient<IPresentationService, PresentationService.API.PresentationService>((sp, c) =>
            {
                c.BaseAddress = new Uri("https://localhost:5011/api/v1/");

                var cookies = sp.GetRequiredService<ICookieStorage>();
                var token = cookies.GetValueAsync(Constants.AUTH_TOKEN_COOKIE).Result;
                if (token.IsNotNullOrEmpty())
                {
                    c.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    c.DefaultRequestHeaders.Add("token", token);
                }
            });
            services.AddHttpClient<IUserService, UserService.API.UserService>((sp, c) =>
            {
                c.BaseAddress = new Uri("https://localhost:5001/api/v1/");

                var cookies = sp.GetRequiredService<ICookieStorage>();
                var token = cookies.GetValueAsync(Constants.AUTH_TOKEN_COOKIE).Result;
                if (token.IsNotNullOrEmpty())
                {
                    c.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    c.DefaultRequestHeaders.Add("token", token);
                }
            });

            services.AddUINecessaryFeatures();

            await builder.Build().RunAsync();
        }
    }
}
