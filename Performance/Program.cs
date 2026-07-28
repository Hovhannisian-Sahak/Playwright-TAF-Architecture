using Performance;

var options = PerformanceOptions.FromArgs(args);
var runner = new PerformanceTestRunner();

var runResult = await runner.RunAsync(options);

if (!runResult.Passed)
{
    Environment.ExitCode = 1;
}
