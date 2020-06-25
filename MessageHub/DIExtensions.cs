using Microsoft.Extensions.Configuration;
using SharedT.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SharedT;
using TestsStorageService.API;
using Utilities.Extensions;
using MessageHub;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DIExtensions
    {
        public static IServiceCollection AddMessagingServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MessageConsumerOptions>(configuration.GetSection("MessagingService"));
            services.AddAttributeRegisteredServices(typeof(IMessageConsumer).Assembly);

            return services;
        }
    }
}
