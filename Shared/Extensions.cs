using Grpc.Core;
using Grpc.Net.Client;
using MessageHub;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using System;
using System.Reflection;
using Utilities.Extensions;

namespace Shared
{
    public static class Extensions
    {
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
            services.AddAttributeRegisteredServices(typeof(IMessageConsumer).Assembly);

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

        
    }
}
