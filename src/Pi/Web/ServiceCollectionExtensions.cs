using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Web.Helpers;
using Web.Hubs;

namespace Web
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSerilog(this IServiceCollection services)
        {
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
            return services.AddSingleton(Log.Logger);
        }

        public static IApplicationBuilder UseSerilog(this IApplicationBuilder app, IHubContext<LogHub> hubContext)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.SignalR(hubContext)
                .CreateLogger();

            return app;
        }
    }
}