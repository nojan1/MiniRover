using System;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Web
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSerilog( this IServiceCollection services)
        {
            Log.Logger =  new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
            return services.AddSingleton(Log.Logger);
        }
    }
}