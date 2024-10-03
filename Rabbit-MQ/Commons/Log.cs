using Microsoft.Extensions.Logging;
using Serilog;

namespace Commons
{
    public static class Log
    {
        public static ILoggingBuilder AddLogs(this ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(
                new LoggerConfiguration()
                .WriteTo.File(
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                    path: "Metrics/output.csv",
                    outputTemplate: "{Timestamp:HH:mm:ss:fff};{Message}{NewLine}",  // Formato customizado
                    rollingInterval: RollingInterval.Day
                )
                .WriteTo.File(
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
                    path: $"Logs/{DateTime.Now:MM-yyyy}/.txt",
                    rollingInterval: RollingInterval.Day
                    )
                .CreateLogger());
            return loggingBuilder;
        }
    }
}
