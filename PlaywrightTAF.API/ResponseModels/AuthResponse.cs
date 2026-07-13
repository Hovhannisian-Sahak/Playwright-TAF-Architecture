namespace PlaywrightTAF.Core.Models;

public class AuthResponse
{
    public UserData user { get; set; } = new();
}

public class UserData
{
    public string username { get; set; } = string.Empty;

    public string email { get; set; } = string.Empty;

    public string token { get; set; } = string.Empty;
}
