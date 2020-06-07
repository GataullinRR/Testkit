using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared;

namespace WebNotificationService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR()
                    .AddNewtonsoftJsonProtocol(options =>
                    {
                        options.PayloadSerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All; // Add one more security breach!
                    });
            services.AddResponseCompression(opts =>
                    {
                        opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
                    });

            services.AddCors();
            services.AddServices();
            services.AddNecessaryFeatures();
            services.AddMessaging(Configuration.GetSection("Messaging"));

            services.AddHostedService<Notifier>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<SignalRHub>("/signalRHub");
            });
        }
    }
}
