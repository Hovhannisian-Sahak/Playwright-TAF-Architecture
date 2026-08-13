using System.Collections.Concurrent;

namespace Performance.Metrics;

public sealed class ApiPerformanceMetrics
{
    private readonly ConcurrentBag<double> _durationsMs = [];
    private int _failures;
    private int _requests;

    public void RecordRequest()
    {
        Interlocked.Increment(ref _requests);
    }

    public void RecordDuration(TimeSpan duration)
    {
        _durationsMs.Add(duration.TotalMilliseconds);
    }

    public void RecordFailure()
    {
        Interlocked.Increment(ref _failures);
    }

    public ApiPerformanceResults GetResults()
    {
        var orderedDurations = _durationsMs.OrderBy(value => value).ToArray();
        var requests = _requests;
        var failures = _failures;

        return new ApiPerformanceResults(
            requests,
            failures,
            requests == 0 ? 1 : (double)failures / requests,
            orderedDurations.Length == 0 ? 0 : orderedDurations.Average(),
            Percentile(orderedDurations, 0.95));
    }

    private static double Percentile(double[] sortedValues, double percentile)
    {
        if (sortedValues.Length == 0)
        {
            return 0;
        }

        var index = (int)Math.Ceiling(percentile * sortedValues.Length) - 1;
        return sortedValues[Math.Clamp(index, 0, sortedValues.Length - 1)];
    }
}
