using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using SharedT.Types;
using System;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using TestsStorageService.API;
using Utilities.Extensions;

namespace SharedT
{
    public static class Extensions
    {
        public static IHostBuilder UseStandardLogging(this IHostBuilder host, string folder, string filePrefix)
        {
            return host.UseSerilog((hostingContext, loggerConfiguration) =>
            {
                var consoleTemplate = "[{Timestamp:HH:mm:ss.fff} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}";
                var fileTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3} {SourceContext}] {Message}{NewLine}{Exception}";
                loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Debug()
                    .WriteTo.RollingFile(Path.Combine("C:\\Logs", folder, filePrefix + "-{Date}.txt"),
                        outputTemplate: fileTemplate,
                        fileSizeLimitBytes: 1024 * 1024 * 1,
                        retainedFileCountLimit: 20)
                    .WriteTo.Console(
                        outputTemplate: consoleTemplate)
                    .MinimumLevel.Verbose()
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information);
            });
        }

        public static IServiceCollection AddTypedHttpClientForBasicService<TService, TImplementation>(
            this IServiceCollection services, 
            IConfiguration configuration)
            where TService : class
            where TImplementation : class, TService
        {
            return services.AddTypedHttpClientForBasicService<TService, TImplementation>(configuration, (sp, c) => { });
        }
        public static IServiceCollection AddTypedHttpClientForBasicService<TService, TImplementation>(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IServiceProvider, HttpClient> configureClient)
            where TService : class
            where TImplementation : class, TService
        {
            var options = configuration.Get<BasicServiceOption>();
            if (options != default)
            {
                services.AddHttpClient<TService, TImplementation>((sp, c) =>
                {
                    c.BaseAddress = options.Address;
                    c.Timeout = options.Timeout;
                    configureClient(sp, c);
                });
            }
            else
            {
                throw new InvalidOperationException("Could not get options");
            }

            return services;
        }

        public static IServiceCollection AddUINecessaryFeatures(this IServiceCollection services)
        {
            services.AddUtilityServices();
            services.AddAttributeRegisteredServices(Assembly.GetCallingAssembly());

            var jsonOptions = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            services.AddSingleton(jsonOptions);

            return services;
        }
        public static IServiceCollection AddNecessaryFeatures(this IServiceCollection services)
        {
            services.AddUtilityServices();
            services.AddAttributeRegisteredServices(Assembly.GetCallingAssembly());
    
            var jsonOptions = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            services.AddSingleton(jsonOptions);

            return services;
        }

        public static IServiceCollection AddDbInitializer<T>(this IServiceCollection services) where T : class
        {
            services.TryAddScoped<T>();
            using var sp = services.BuildServiceProvider();
            sp.GetRequiredService<T>();

            return services;
        }
    }
}
