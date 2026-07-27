using System.Diagnostics;
using Performance.Clients;
using Performance.Metrics;

namespace Performance.Scenarios;

public sealed class CreateArticleScenario
{
    private readonly ConduitApiClient _apiClient;
    private readonly PerformanceMetrics _metrics;
    private readonly TimeSpan _requestDelay;

    public CreateArticleScenario(
        ConduitApiClient apiClient,
        PerformanceMetrics metrics,
        TimeSpan requestDelay)
    {
        _apiClient = apiClient;
        _metrics = metrics;
        _requestDelay = requestDelay;
    }

    public async Task RunVirtualUserAsync(int virtualUser, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _apiClient.RegisterUserAsync(virtualUser, cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                await RunCreateArticleIterationAsync(user.Token, virtualUser, cancellationToken);
                await Task.Delay(_requestDelay, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
    }

    private async Task RunCreateArticleIterationAsync(
        string token,
        int virtualUser,
        CancellationToken cancellationToken)
    {
        _metrics.RecordRequest();

        try
        {
            var stopwatch = Stopwatch.StartNew();
            var slug = await _apiClient.CreateArticleAsync(token, virtualUser, cancellationToken);
            stopwatch.Stop();

            _metrics.RecordDuration(stopwatch.Elapsed);
            await _apiClient.DeleteArticleAsync(token, slug, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            _metrics.RecordFailure();
            Console.Error.WriteLine($"VU {virtualUser} failed: {ex.Message}");
        }
    }
}
