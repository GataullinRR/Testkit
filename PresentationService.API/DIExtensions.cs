using Microsoft.Extensions.Configuration;
using SharedT.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using PresentationService.API;
using SharedT;
using Utilities.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DIExtensions
    {
        public static IServiceCollection AddPresentationService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAttributeRegisteredServices();
            var options = configuration.GetSection("PresentationService");

            return services.AddTypedHttpClientForBasicService<
                IPresentationService, 
                PresentationService.API.PresentationService>(options, async (sp, c) => 
                {
                    var token = await sp.GetRequiredService<ITokenProvider>().GetTokenAsync();
                    if (token.IsNotNullOrEmpty())
                    {
                        c.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        c.DefaultRequestHeaders.Add("token", token);
                    }
                });
        }
    }
}
