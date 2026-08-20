namespace PlaywrightTAF.Core.Authentication;

public static class AuthStatePaths
{
    private static readonly string Root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Authentication", "AuthStates"));
    private static readonly string RunId = $"{Environment.ProcessId}-{Guid.NewGuid():N}";

    public static string Admin => Path.Combine(Root, "adminState.json");

    public static string User => Path.Combine(Root, "userState.json");

    public static string CurrentRunAdmin => Path.Combine(Root, $"adminState-{RunId}.json");

    public static string CurrentRunUser => Path.Combine(Root, $"userState-{RunId}.json");

    public static void EnsureDirectoryExists()
    {
        Directory.CreateDirectory(Root);
    }
}
