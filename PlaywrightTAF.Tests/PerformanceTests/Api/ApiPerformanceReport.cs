using System.IO;
using NUnit.Framework;
using Performance;

namespace PlaywrightTAF.Tests.PerformanceTests.Api;

internal static class ApiPerformanceReport
{
    public static void WriteOutput(StringWriter output, StringWriter error)
    {
        TestContext.Progress.WriteLine(output.ToString());

        var errorText = error.ToString();
        if (!string.IsNullOrWhiteSpace(errorText))
        {
            TestContext.Error.WriteLine(errorText);
        }
    }

    public static void AttachResults(ApiPerformanceRunResult runResult)
    {
        PerformanceReportAttachment.AddJson(
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
