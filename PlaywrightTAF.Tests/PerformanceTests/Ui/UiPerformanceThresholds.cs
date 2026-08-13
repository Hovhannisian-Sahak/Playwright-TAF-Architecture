using System;

namespace PlaywrightTAF.Tests.PerformanceTests.Ui;

internal sealed record UiPerformanceThresholds(
    double MaxPageReadyMs,
    double MaxBrowserLoadMs)
{
    private const double DefaultMaxPageReadyMs = 30000;
    private const double DefaultMaxBrowserLoadMs = 10000;

    public static UiPerformanceThresholds FromEnvironment(string pageName)
    {
        string pagePrefix = ToEnvironmentName(pageName);

        return new UiPerformanceThresholds(
            GetDouble(
                $"UI_PERF_{pagePrefix}_MAX_PAGE_READY_MS",
                GetDouble("UI_PERF_MAX_PAGE_READY_MS", DefaultMaxPageReadyMs)),
            GetDouble(
                $"UI_PERF_{pagePrefix}_MAX_BROWSER_LOAD_MS",
                GetDouble("UI_PERF_MAX_BROWSER_LOAD_MS", DefaultMaxBrowserLoadMs)));
    }

    public static string ToEnvironmentName(string value)
    {
        return value
            .Replace(" ", "_", StringComparison.OrdinalIgnoreCase)
            .ToUpperInvariant();
    }

    private static double GetDouble(string name, double defaultValue)
    {
        string? value = Environment.GetEnvironmentVariable(name);

        return double.TryParse(value, out double parsedValue)
            ? parsedValue
            : defaultValue;
    }
}
