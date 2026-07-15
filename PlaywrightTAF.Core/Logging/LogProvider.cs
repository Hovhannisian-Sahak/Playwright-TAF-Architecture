using Serilog;
using Serilog.Events;

namespace PlaywrightTAF.Core.Logging;

public static class LogProvider
{
    private const string DefaultLogLevel = "Information";
    private static readonly Lazy<ILogger> Logger = new(CreateLogger);

    public static ILogger Current => Logger.Value;

    public static ILogger ForContext<T>()
    {
        return Current.ForContext<T>();
    }

    private static ILogger CreateLogger()
    {
        string levelName = Environment.GetEnvironmentVariable("TAF_LOG_LEVEL") ?? DefaultLogLevel;

        if (!Enum.TryParse(levelName, ignoreCase: true, out LogEventLevel minimumLevel))
        {
            minimumLevel = LogEventLevel.Information;
        }

        return new LoggerConfiguration()
            .MinimumLevel.Is(minimumLevel)
            .Enrich.FromLogContext()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/test-run-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 10,
                shared: true,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] [P:{ProcessId} T:{ThreadId}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }
}
