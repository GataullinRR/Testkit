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

namespace Runner
{
    public class Program
    {
        public const string SERVER = "172.18.57.206";

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            var services = builder.Services;
            services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            services.AddHttpClient<IPresentationService, PresentationService.API.PresentationService>(async (sp, c) =>
            {
                c.BaseAddress = new Uri($"https://{SERVER}:5011/api/v1/");

                var cookies = sp.GetRequiredService<ICookieStorage>();
                var token = await cookies.GetValueAsync(Constants.AUTH_TOKEN_COOKIE);
                if (token.IsNotNullOrEmpty())
                {
                    c.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    c.DefaultRequestHeaders.Add("token", token);
                }
            });
            services.AddHttpClient<IUserService, UserService.API.UserService>(async (sp, c) =>
            {
                c.BaseAddress = new Uri($"https://{SERVER}:5015/api/v1/");

                var cookies = sp.GetRequiredService<ICookieStorage>();
                var token = await cookies.GetValueAsync(Constants.AUTH_TOKEN_COOKIE);
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
