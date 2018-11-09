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
        private readonly IHubContext<LogHub> _hubContext;

        public SerilogSignalRBridge(IFormatProvider formatProvider, IHubContext<LogHub> hubContext)
        {
            _formatProvider = formatProvider;
            _hubContext = hubContext;
        }

        public async void Emit(LogEvent logEvent)
        {
            await _hubContext.Clients.All.SendAsync("LogEventEmitted", new {
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
                  IHubContext<LogHub> hubContext,
                  IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new SerilogSignalRBridge(formatProvider, hubContext));
        }
    }
}