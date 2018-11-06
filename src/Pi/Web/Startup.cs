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
using Rebus.Config;
using Rebus;
using Rebus.Transport.InMem;
using Core.Runtime;
using System.Threading.Tasks;
using Web.Hubs;

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

            builder.RegisterHandler<DataHubBusAdapter>();

            ConfigureRebus(builder);
            Core.Bootstrap.Configure(builder);
            Container = builder.Build();
            return new AutofacServiceProvider(Container);
        }

        private void OnShutdown()
        {
            _serviceRunner.Stop();
            Container.Dispose();
        }

        private void ConfigureRebus(ContainerBuilder builder)
        {
            var network = new InMemNetwork();

            builder.RegisterRebus((configurer, context) => configurer
                //.Logging(l => l.ColoredConsole())
                .Transport(t => t.UseInMemoryTransport(network, "inputque"))
                .Options(o =>
                {
                    o.SetNumberOfWorkers(2);
                    o.SetMaxParallelism(30);
                }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime, ServiceRunner serviceRunner)
        {
            _serviceRunner = serviceRunner;
            applicationLifetime.ApplicationStopping.Register(OnShutdown);

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
