using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MessageHub;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;
using TestsStorageService;
using TestsStorageService.Db;
using UserService;
using Utilities.Extensions;

namespace TestsStorage
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TestsContext>(options =>
                options.UseSqlServer(Configuration.GetSection("DefaultConnection").Value));

            services.AddGrpc();
            services.AddNecessaryFeatures();
            services.AddGrpcServices();
            services.AddMessaging(Configuration.GetSection("Messaging"));

            services.AddDbInitializer<DbSeeder>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSingletonInitialization();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcServices(Assembly.GetExecutingAssembly(),
                    typeof(GrpcEndpointRouteBuilderExtensions).GetMethod("MapGrpcService", BindingFlags.Static | BindingFlags.Public));
            });
        }
    }
}