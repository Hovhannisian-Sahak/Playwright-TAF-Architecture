using System.IO;
using System.Threading.Tasks;
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
        var options = ApiPerformanceOptions.FromEnvironment();
        var output = new StringWriter();
        var error = new StringWriter();

        var runResult = await new PerformanceTestRunner().RunAsync(options, output, error);

        WriteRunOutput(output, error);
        AddAllureResultsAttachment(runResult);

        ApiPerformanceAssertions.ShouldMeetThresholds(runResult);
    }

    private static void WriteRunOutput(StringWriter output, StringWriter error)
    {
        TestContext.Progress.WriteLine(output.ToString());

        var errorText = error.ToString();
        if (!string.IsNullOrWhiteSpace(errorText))
        {
            TestContext.Error.WriteLine(errorText);
        }
    }

    private static void AddAllureResultsAttachment(PerformanceRunResult runResult)
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
