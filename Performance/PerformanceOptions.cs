namespace Performance;

public sealed class PerformanceOptions
{
    public string BaseUrl { get; private init; } = "https://conduit-api.bondaracademy.com";

    public int VirtualUsers { get; private init; } = 3;

    public TimeSpan Duration { get; private init; } = TimeSpan.FromSeconds(30);

    public TimeSpan RequestDelay { get; private init; } = TimeSpan.FromSeconds(1);

    public double MaxP95Ms { get; private init; } = 1000;

    public double MaxFailureRate { get; private init; } = 0.01;

    public static PerformanceOptions FromArgs(string[] args)
    {
        return new PerformanceOptions
        {
            BaseUrl = GetArg(args, "--base-url", "https://conduit-api.bondaracademy.com"),
            VirtualUsers = int.Parse(GetArg(args, "--vus", "3")),
            Duration = TimeSpan.FromSeconds(int.Parse(GetArg(args, "--duration-seconds", "30"))),
            RequestDelay = TimeSpan.FromSeconds(int.Parse(GetArg(args, "--request-delay-seconds", "1"))),
            MaxP95Ms = double.Parse(GetArg(args, "--max-p95-ms", "1000")),
            MaxFailureRate = double.Parse(GetArg(args, "--max-failure-rate", "0.01"))
        };
    }

    private static string GetArg(string[] args, string name, string defaultValue)
    {
        var index = Array.IndexOf(args, name);
        return index >= 0 && index + 1 < args.Length ? args[index + 1] : defaultValue;
    }
}
