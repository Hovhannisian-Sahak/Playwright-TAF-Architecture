
namespace PlaywrightTAF.Core.Authentication;

public static class AuthStatePaths
{
    private static readonly string Root =
        Path.Combine(
            Directory.GetCurrentDirectory(),
            "..",
            "..",
            "..",
            "Authentication",
            "AuthStates");


    public static string Admin =>
        Path.Combine(Root, "adminState.json");


    public static string User =>
        Path.Combine(Root, "userState.json");
}