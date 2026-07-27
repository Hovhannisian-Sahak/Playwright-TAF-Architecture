using Performance;
using Performance.Clients;
using Performance.Metrics;
using Performance.Scenarios;

var options = PerformanceOptions.FromArgs(args);

using var httpClient = new HttpClient
{
    BaseAddress = new Uri(options.BaseUrl)
};

var apiClient = new ConduitApiClient(httpClient);
var metrics = new PerformanceMetrics();
var scenario = new CreateArticleScenario(apiClient, metrics, options.RequestDelay);

using var cancellation = new CancellationTokenSource(options.Duration);

Console.WriteLine(
    $"Running POST /api/articles performance test: vus={options.VirtualUsers}, duration={options.Duration.TotalSeconds}s, baseUrl={options.BaseUrl}");

var tasks = Enumerable
    .Range(1, options.VirtualUsers)
    .Select(virtualUser => scenario.RunVirtualUserAsync(virtualUser, cancellation.Token))
    .ToArray();

await Task.WhenAll(tasks);

var results = metrics.GetResults();

Console.WriteLine();
Console.WriteLine("Results");
Console.WriteLine($"Requests: {results.Requests}");
Console.WriteLine($"Failures: {results.Failures}");
Console.WriteLine($"Failure rate: {results.FailureRate:P2}");
Console.WriteLine($"Average POST duration: {results.AverageMs:N0} ms");
Console.WriteLine($"P95 POST duration: {results.P95Ms:N0} ms");

if (results.FailureRate > options.MaxFailureRate || results.P95Ms > options.MaxP95Ms)
{
    Console.Error.WriteLine(
        $"Performance thresholds failed. Expected failure rate <= {options.MaxFailureRate:P2}, p95 <= {options.MaxP95Ms:N0} ms.");
    Environment.ExitCode = 1;
}
