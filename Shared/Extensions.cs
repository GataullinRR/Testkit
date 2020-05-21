﻿using Grpc.Core;
using Grpc.Net.Client;
using MessageHub;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PresentationService.API;
using System;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using Utilities.Extensions;

namespace Shared
{
    public static class Extensions
    {
        public static IHostBuilder ConfigureHost(this IHostBuilder builder)
        {
            return builder.ConfigureLogging(logBuilder =>
            {
                // clear default logging providers
                logBuilder.ClearProviders();

                // add built-in providers manually, as needed 
                logBuilder.AddConsole();
                logBuilder.AddDebug();
                logBuilder.AddEventLog();
                logBuilder.AddEventSourceLogger();

                //logBuilder.AddTraceSource("Information, ActivityTracing"); // Add Trace listener provider
            });
        }

        public static IServiceCollection AddUINecessaryFeatures(this IServiceCollection services)
        {
            services.AddUtilityServices();
            services.AddAttributeRegisteredServices(Assembly.GetCallingAssembly());
            services.AddAttributeRegisteredServices(typeof(IWebMessageHub).Assembly);

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
            services.AddAttributeRegisteredServices(typeof(IMessageConsumer).Assembly);
            services.AddLogging(config => config.AddConsole());
    
            var jsonOptions = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            services.AddSingleton(jsonOptions);

            return services;
        }

        public static IServiceCollection AddGrpcServices(this IServiceCollection services)
        {
            services.AddGrpcService<UserService.API.UserService.UserServiceClient>("https://localhost:5001/");
            services.AddGrpcService<TestsStorageService.API.TestsStorageService.TestsStorageServiceClient>("https://localhost:5020");
            services.AddGrpcService<TestsSourceService.API.TestsSourceService.TestsSourceServiceClient>("https://localhost:5041");
            services.AddGrpcService<RunnerService.API.RunnerService.RunnerServiceClient>("https://localhost:5031");

            return services;
        }

        public static IServiceCollection AddGrpcService<T>(this IServiceCollection services, string address) where T : ClientBase
        {
            var channel = GrpcChannel.ForAddress(address);
            var service = Activator.CreateInstance(typeof(T), channel).To<T>();
            services.AddSingleton(service);

            return services;
        }

        public static IServiceCollection AddDbInitializer<T>(this IServiceCollection services) where T : class
        {
            services.TryAddSingleton<T>();
            using var sp = services.BuildServiceProvider();
            sp.GetRequiredService<T>();

            return services;
        }

        //public static IApplicationBuilder UseGrpcEndpoints(this IApplicationBuilder app)
        //{
        //    var target = Assembly.GetCallingAssembly();

        //    app.GetType().GetMethod("UseEndpoints").Invoke(app, )
        //    app.UseEndpoints(endpoints =>
        //    {
        //        endpoints.MapGrpcServices(Assembly.GetCallingAssembly());
        //    });

        //    return app;

        //    void map<T>(T rrr)
        //    {
        //        rrr.MapGrpcServices(rrr, target);
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder">Must be IEndpointRouteBuilder!</param>
        /// <param name="assembly"></param>
        public static void MapGrpcServices(this object builder, Assembly assembly, MethodInfo method)
        {
            var services = assembly.DefinedTypes
                .Where(t => t.GetCustomAttribute<GrpcServiceAttribute>() != null);
            foreach (var service in services)
            {
                method
                    .MakeGenericMethod(service)
                    .Invoke(null, new[] { builder });
            }
        }
    }
}
