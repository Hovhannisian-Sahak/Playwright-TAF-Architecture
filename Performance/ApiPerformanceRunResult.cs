using Performance.Metrics;

namespace Performance;

public sealed record ApiPerformanceRunResult(
    ApiPerformanceOptions Options,
    ApiPerformanceResults Results)
{
    public bool Passed =>
        Results.FailureRate <= Options.MaxFailureRate
        && Results.P95DurationMs <= Options.MaxP95Ms;

    public string ThresholdFailureMessage =>
        $"Performance thresholds failed. Expected failure rate <= {Options.MaxFailureRate:P2}, p95 <= {Options.MaxP95Ms:N0} ms.";
}
