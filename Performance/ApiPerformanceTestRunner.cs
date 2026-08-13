using Performance.Clients;
using Performance.Metrics;
using Performance.Scenarios;

namespace Performance;

public sealed class ApiPerformanceTestRunner
{
    public async Task<ApiPerformanceRunResult> RunAsync(
        ApiPerformanceOptions options,
        TextWriter? output = null,
        TextWriter? error = null)
    {
        output ??= Console.Out;
        error ??= Console.Error;

        using var httpClient = new HttpClient
        {
            BaseAddress = new Uri(options.BaseUrl)
        };

        var apiClient = new ConduitApiClient(httpClient);
        var metrics = new ApiPerformanceMetrics();
        var scenario = new CreateArticleLoadScenario(apiClient, metrics, options.RequestDelay);

        using var cancellation = new CancellationTokenSource(options.Duration);

        await output.WriteLineAsync(
            $"Running POST /api/articles performance test: vus={options.VirtualUsers}, duration={options.Duration.TotalSeconds}s, baseUrl={options.BaseUrl}");

        var tasks = Enumerable
            .Range(1, options.VirtualUsers)
            .Select(virtualUser => scenario.RunVirtualUserAsync(virtualUser, cancellation.Token))
            .ToArray();

        await Task.WhenAll(tasks);

        var runResult = new ApiPerformanceRunResult(options, metrics.GetResults());

        await WriteResultsAsync(output, runResult.Results);

        if (!runResult.Passed)
        {
            await error.WriteLineAsync(runResult.ThresholdFailureMessage);
        }

        return runResult;
    }

    private static async Task WriteResultsAsync(TextWriter output, ApiPerformanceResults results)
    {
        await output.WriteLineAsync();
        await output.WriteLineAsync("Results");
        await output.WriteLineAsync($"Requests: {results.TotalRequests}");
        await output.WriteLineAsync($"Failures: {results.FailedRequests}");
        await output.WriteLineAsync($"Failure rate: {results.FailureRate:P2}");
        await output.WriteLineAsync($"Average POST duration: {results.AverageDurationMs:N0} ms");
        await output.WriteLineAsync($"P95 POST duration: {results.P95DurationMs:N0} ms");
    }
}
