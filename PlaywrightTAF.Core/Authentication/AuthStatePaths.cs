
namespace PlaywrightTAF.Core.Authentication;

public static class AuthStatePaths
{
    private static readonly string Root =
        Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "Authentication",
                "AuthStates"));


    public static string Admin =>
        Path.Combine(Root, "adminState.json");


    public static string User =>
        Path.Combine(Root, "userState.json");

    public static void EnsureDirectoryExists()
    {
        Directory.CreateDirectory(Root);
    }
}
