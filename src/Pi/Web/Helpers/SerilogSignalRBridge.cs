using System;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Web.Hubs;

namespace Web.Helpers
{
    public class SerilogSignalRBridge : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;

        public static IHubContext<LogHub> HubContext { get; set; }

        public SerilogSignalRBridge(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
        }

        public async void Emit(LogEvent logEvent)
        {
            if(HubContext == null)
                return;

            await HubContext.Clients.All.SendAsync("LogEventEmitted", new {
                Properties = logEvent.Properties,
                Level = logEvent.Level.ToString(),
                ExceptionMessage = logEvent.Exception?.Message,
                Message = logEvent.RenderMessage(_formatProvider)
            });
        }
    }

    public static class MySinkExtensions
    {
        public static LoggerConfiguration SignalR(
                  this LoggerSinkConfiguration loggerConfiguration,
                  IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new SerilogSignalRBridge(formatProvider));
        }
    }
}