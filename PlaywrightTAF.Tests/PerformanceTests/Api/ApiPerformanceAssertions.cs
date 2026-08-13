using NUnit.Framework;
using Performance;

namespace PlaywrightTAF.Tests.PerformanceTests.Api;

internal static class ApiPerformanceAssertions
{
    public static void ShouldMeetThresholds(ApiPerformanceRunResult runResult)
    {
        Assert.Multiple(() =>
        {
            Assert.That(
                runResult.Results.FailureRate,
                Is.LessThanOrEqualTo(runResult.Options.MaxFailureRate),
                $"Failure rate should be <= {runResult.Options.MaxFailureRate:P2}.");

            Assert.That(
                runResult.Results.P95DurationMs,
                Is.LessThanOrEqualTo(runResult.Options.MaxP95Ms),
                $"P95 duration should be <= {runResult.Options.MaxP95Ms:N0} ms.");
        });
    }
}
