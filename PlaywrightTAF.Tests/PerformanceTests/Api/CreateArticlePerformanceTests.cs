using System;
using System.Collections.Generic;
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
    [Category("API")]
    [Category("Performance")]
    [Category("APIPerformance")]
    public async Task CreateArticle_ShouldMeetPerformanceThresholds()
    {
        var options = PerformanceOptions.FromArgs(CreateArgsFromEnvironment());
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

        Assert.Multiple(() =>
        {
            Assert.That(
                runResult.Results.FailureRate,
                Is.LessThanOrEqualTo(options.MaxFailureRate),
                $"Failure rate should be <= {options.MaxFailureRate:P2}.");

            Assert.That(
                runResult.Results.P95Ms,
                Is.LessThanOrEqualTo(options.MaxP95Ms),
                $"P95 duration should be <= {options.MaxP95Ms:N0} ms.");
        });
    }

    private static string[] CreateArgsFromEnvironment()
    {
        var args = new List<string>();

        AddArg(args, "--base-url", "PERF_BASE_URL");
        AddArg(args, "--vus", "PERF_VUS");
        AddArg(args, "--duration-seconds", "PERF_DURATION_SECONDS");
        AddArg(args, "--request-delay-seconds", "PERF_REQUEST_DELAY_SECONDS");
        AddArg(args, "--max-p95-ms", "PERF_MAX_P95_MS");
        AddArg(args, "--max-failure-rate", "PERF_MAX_FAILURE_RATE");

        return args.ToArray();
    }

    private static void AddArg(List<string> args, string optionName, string environmentVariableName)
    {
        var value = Environment.GetEnvironmentVariable(environmentVariableName);

        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        args.Add(optionName);
        args.Add(value);
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
