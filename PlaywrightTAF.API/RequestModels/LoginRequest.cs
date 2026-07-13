namespace PlaywrightTAF.Core.RequestModels;

public class LoginRequest
{
    public UserLogin user { get; set; } = new();
}


public class UserLogin
{
    public string email { get; set; } = string.Empty;

    public string password { get; set; } = string.Empty;
}