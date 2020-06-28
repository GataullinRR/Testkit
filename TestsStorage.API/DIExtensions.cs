using Microsoft.Extensions.Configuration;
using SharedT.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SharedT;
using TestsStorageService.API;
using Utilities.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DIExtensions
    {
        public static IServiceCollection AddTestsStorageService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAttributeRegisteredServices();
            var options = configuration.GetSection("TestsStorageService");

            return services.AddTypedHttpClientForBasicService<
                ITestsStorageService,
                TestsStorageService.API.TestsStorageService>(options);
        }
    }
}
