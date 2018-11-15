using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Runtime;
using System.Threading.Tasks;
using Web.Hubs;
using Serilog;
using Web.Models;
using Microsoft.AspNetCore.SignalR;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Startup(IContainer container, IConfiguration configuration)
        {
            this.Container = container;
            this.Configuration = configuration;
        }
        public IContainer Container { get; private set; }
        public IConfiguration Configuration { get; }

        private ServiceRunner _serviceRunner;

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSerilog();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSignalR();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            // Create the container builder.
            var builder = new ContainerBuilder();
            builder.Populate(services);

            builder.RegisterType<DataHubBusAdapter>().AsImplementedInterfaces();

            var coreConfiguration = new CoreConfiguration();
            Configuration.Bind("CoreConfiguration", coreConfiguration);
            Core.Bootstrap.Configure(builder, new ConfigurationProgramAssemblyProvider(Configuration), coreConfiguration);

            var cameraConfiguration = new CameraConfiguration();
            Configuration.Bind("CameraConfiguration", cameraConfiguration);
            builder.Register(x => cameraConfiguration).AsImplementedInterfaces();
            
            Container = builder.Build();
            return new AutofacServiceProvider(Container);
        }

        private void OnShutdown()
        {
            _serviceRunner.Stop();
            Container.Dispose();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime, ServiceRunner serviceRunner, IHubContext<LogHub> hubContext)
        {
            _serviceRunner = serviceRunner;
            applicationLifetime.ApplicationStopping.Register(OnShutdown);

            Helpers.SerilogSignalRBridge.HubContext = hubContext;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseSignalR(routes =>
            {
                routes.MapHub<DataHub>("/data");
                routes.MapHub<LogHub>("/log");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            _serviceRunner.Start();
        }
    }
}
