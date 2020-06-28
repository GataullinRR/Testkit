using Microsoft.Extensions.Configuration;
using SharedT.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SharedT;
using TestsStorageService.API;
using UserService.API;
using Utilities.Extensions;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DIExtensions
    {
        public static IServiceCollection AddUserService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAttributeRegisteredServices();
            var options = configuration.GetSection("UserService");

            return services.AddTypedHttpClientForBasicService<
                IUserService,
                UserService.API.UserService>(options, async (sp, c) => 
                {
                    var token = await (sp.GetService<ITokenProvider>()?.GetTokenAsync() ?? Task.FromResult((string?)null));
                    if (token.IsNotNullOrEmpty())
                    {
                        c.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        c.DefaultRequestHeaders.Add("token", token);
                    }
                });
        }
    }
}
