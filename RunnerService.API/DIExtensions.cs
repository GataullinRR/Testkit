using Microsoft.Extensions.Configuration;
using SharedT.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SharedT;
using RunnerService.API;
using Utilities.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DIExtensions
    {
        public static IServiceCollection AddRunnerService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAttributeRegisteredServices();
            var options = configuration.GetSection("RunnerService");

            return services.AddTypedHttpClientForBasicService<
                IRunnerService,
                RunnerService.API.RunnerService>(options);
        }
    }
}
