using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared;
using StateService.Db;
using Utilities.Extensions;

namespace StateService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<StateContext>(options =>
                options.UseSqlServer(Configuration.GetSection("DefaultConnection").Value));

            services.AddGrpc();
            services.AddNecessaryFeatures();
            //services.AddGrpcServices();
            services.AddMessaging(Configuration.GetSection("Messaging"));

            services.AddHostedService<MessageHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseSingletonInitialization();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GrpcService>(); // it uses lock(){} inside!
                //endpoints.MapGrpcServices(Assembly.GetExecutingAssembly(),
                //    typeof(GrpcEndpointRouteBuilderExtensions).GetMethod("MapGrpcService", BindingFlags.Static | BindingFlags.Public));
            });
        }
    }


}
