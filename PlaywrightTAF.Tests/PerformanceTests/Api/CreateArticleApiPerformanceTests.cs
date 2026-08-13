using System.IO;
using System.Threading.Tasks;
using Allure.NUnit;
using NUnit.Framework;
using Performance;

namespace PlaywrightTAF.Tests.PerformanceTests.Api;

[AllureNUnit]
[TestFixture]
public sealed class CreateArticleApiPerformanceTests
{
    [Test]
    [Category("Performance")]
    [Category("APIPerformance")]
    public async Task CreateArticle_ShouldMeetPerformanceThresholds()
    {
        var options = ApiPerformanceOptions.FromEnvironment();
        var output = new StringWriter();
        var error = new StringWriter();

        var runResult = await new ApiPerformanceTestRunner().RunAsync(options, output, error);

        ApiPerformanceReport.WriteOutput(output, error);
        ApiPerformanceReport.AttachResults(runResult);

        ApiPerformanceAssertions.ShouldMeetThresholds(runResult);
    }
}
