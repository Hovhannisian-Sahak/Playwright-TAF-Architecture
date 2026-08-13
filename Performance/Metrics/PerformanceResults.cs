namespace Performance.Metrics;

public sealed record PerformanceResults(
    int TotalRequests,
    int FailedRequests,
    double FailureRate,
    double AverageDurationMs,
    double P95DurationMs);
