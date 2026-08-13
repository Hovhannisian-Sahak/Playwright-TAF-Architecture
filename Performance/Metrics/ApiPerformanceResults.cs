namespace Performance.Metrics;

public sealed record ApiPerformanceResults(
    int TotalRequests,
    int FailedRequests,
    double FailureRate,
    double AverageDurationMs,
    double P95DurationMs);
