using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.NUnit;
using NUnit.Framework;
using Performance;

namespace PlaywrightTAF.Tests.PerformanceTests.Api;

[AllureNUnit]
[TestFixture]
public sealed class CreateArticlePerformanceTests
{
    [Test]
    [Category("Performance")]
    [Category("APIPerformance")]
    public async Task CreateArticle_ShouldMeetPerformanceThresholds()
    {
        var options = PerformanceOptions.FromEnvironment();
        var output = new StringWriter();
        var error = new StringWriter();

        var runResult = await new PerformanceTestRunner().RunAsync(options, output, error);

        TestContext.Progress.WriteLine(output.ToString());

        var errorText = error.ToString();
        if (!string.IsNullOrWhiteSpace(errorText))
        {
            TestContext.Error.WriteLine(errorText);
        }

        AddAllureResultsAttachment(runResult);

        AssertPerformanceThresholds(runResult);
    }

    private static void AssertPerformanceThresholds(PerformanceRunResult runResult)
    {
        Assert.Multiple(() =>
        {
            Assert.That(
                runResult.Results.FailureRate,
                Is.LessThanOrEqualTo(runResult.Options.MaxFailureRate),
                $"Failure rate should be <= {runResult.Options.MaxFailureRate:P2}.");

            Assert.That(
                runResult.Results.P95Ms,
                Is.LessThanOrEqualTo(runResult.Options.MaxP95Ms),
                $"P95 duration should be <= {runResult.Options.MaxP95Ms:N0} ms.");
        });
    }

    private static void AddAllureResultsAttachment(PerformanceRunResult runResult)
    {
        var json = JsonSerializer.Serialize(
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
            },
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        AllureApi.AddAttachment(
            "api-performance-results",
            "application/json",
            Encoding.UTF8.GetBytes(json),
            ".json");
    }
}
