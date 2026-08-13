using Performance;

var options = ApiPerformanceOptions.FromArgs(args);
var runner = new ApiPerformanceTestRunner();

var runResult = await runner.RunAsync(options);

if (!runResult.Passed)
{
    Environment.ExitCode = 1;
}
