using Performance;

namespace PlaywrightTAF.Tests.PerformanceTests.Api;

internal static class ApiPerformanceReport
{
    public static void AttachResults(PerformanceRunResult runResult)
    {
        PerformanceAttachment.AddJson(
            "api-performance-results",
            new
            {
                options = new
                {
                    runResult.Options.BaseUrl,
                    runResult.Options.VirtualUsers,
                    durationSeconds = runResult.Options.Duration.TotalSeconds,
                    requestDelaySeconds = runResult.Options.RequestDelay.TotalSeconds,
                    runResult.Options.MaxP95Ms,
                    runResult.Options.MaxFailureRate
                },
                results = runResult.Results,
                passed = runResult.Passed
            });
    }
}
