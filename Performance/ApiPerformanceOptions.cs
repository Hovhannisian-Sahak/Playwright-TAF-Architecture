using System.Globalization;

namespace Performance;

public sealed class ApiPerformanceOptions
{
    private const string DefaultBaseUrl = "https://conduit-api.bondaracademy.com";
    private const string DefaultVirtualUsers = "3";
    private const string DefaultDurationSeconds = "30";
    private const string DefaultRequestDelaySeconds = "1";
    private const string DefaultMaxP95Ms = "1000";
    private const string DefaultMaxFailureRate = "0.01";

    public string BaseUrl { get; private init; } = DefaultBaseUrl;

    public int VirtualUsers { get; private init; } = 3;

    public TimeSpan Duration { get; private init; } = TimeSpan.FromSeconds(30);

    public TimeSpan RequestDelay { get; private init; } = TimeSpan.FromSeconds(1);

    public double MaxP95Ms { get; private init; } = 1000;

    public double MaxFailureRate { get; private init; } = 0.01;

    public static ApiPerformanceOptions FromArgs(string[] args)
    {
        return new ApiPerformanceOptions
        {
            BaseUrl = GetArg(args, "--base-url", DefaultBaseUrl),
            VirtualUsers = int.Parse(GetArg(args, "--vus", DefaultVirtualUsers), CultureInfo.InvariantCulture),
            Duration = TimeSpan.FromSeconds(int.Parse(GetArg(args, "--duration-seconds", DefaultDurationSeconds), CultureInfo.InvariantCulture)),
            RequestDelay = TimeSpan.FromSeconds(int.Parse(GetArg(args, "--request-delay-seconds", DefaultRequestDelaySeconds), CultureInfo.InvariantCulture)),
            MaxP95Ms = double.Parse(GetArg(args, "--max-p95-ms", DefaultMaxP95Ms), CultureInfo.InvariantCulture),
            MaxFailureRate = double.Parse(GetArg(args, "--max-failure-rate", DefaultMaxFailureRate), CultureInfo.InvariantCulture)
        };
    }

    public static ApiPerformanceOptions FromEnvironment()
    {
        return new ApiPerformanceOptions
        {
            BaseUrl = GetEnvironmentValue("PERF_BASE_URL", DefaultBaseUrl),
            VirtualUsers = int.Parse(GetEnvironmentValue("PERF_VUS", DefaultVirtualUsers), CultureInfo.InvariantCulture),
            Duration = TimeSpan.FromSeconds(int.Parse(GetEnvironmentValue("PERF_DURATION_SECONDS", DefaultDurationSeconds), CultureInfo.InvariantCulture)),
            RequestDelay = TimeSpan.FromSeconds(int.Parse(GetEnvironmentValue("PERF_REQUEST_DELAY_SECONDS", DefaultRequestDelaySeconds), CultureInfo.InvariantCulture)),
            MaxP95Ms = double.Parse(GetEnvironmentValue("PERF_MAX_P95_MS", DefaultMaxP95Ms), CultureInfo.InvariantCulture),
            MaxFailureRate = double.Parse(GetEnvironmentValue("PERF_MAX_FAILURE_RATE", DefaultMaxFailureRate), CultureInfo.InvariantCulture)
        };
    }

    private static string GetArg(string[] args, string name, string defaultValue)
    {
        var index = Array.IndexOf(args, name);
        return index >= 0 && index + 1 < args.Length ? args[index + 1] : defaultValue;
    }

    private static string GetEnvironmentValue(string name, string defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
    }
}
