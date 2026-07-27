namespace Performance.Metrics;

public sealed record PerformanceResults(
    int Requests,
    int Failures,
    double FailureRate,
    double AverageMs,
    double P95Ms);
