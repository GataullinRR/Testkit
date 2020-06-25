using Microsoft.Extensions.Configuration;
using SharedT.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SharedT;
using TestsStorageService.API;
using WebNotificationService.API;
using Utilities.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DIExtensions
    {
        public static IServiceCollection AddWebNotificationService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAttributeRegisteredServices();
            services.Configure<WebMessagingOptions>(configuration.GetSection("WebNotificationService"));

            return services;
        }
    }
}
