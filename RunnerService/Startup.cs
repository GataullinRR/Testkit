using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MessageHub;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RunnerService.Db;
using SharedT;
using Utilities.Extensions;
using Utilities.Types;

namespace RunnerService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<RunnerContext>(options =>
                options.UseSqlServer(Configuration.GetSection("DefaultConnection").Value));
            services.AddControllers()
                    .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                        options.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
                    });

            services.AddHostedService<StateCleanerDaemon>();
            services.AddHostedService<RunStateUpdateDaemon>();

            services.AddNecessaryFeatures();
            services.AddTestsStorageService(Configuration.GetSection("Services"));
            services.AddMessagingServices(Configuration.GetSection("Services"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSingletonInitialization();
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
