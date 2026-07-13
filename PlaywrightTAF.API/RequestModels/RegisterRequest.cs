namespace PlaywrightTAF.Core.RequestModels;

public class RegisterRequest
{
    public UserRegister user { get; set; } = new();
}


public class UserRegister
{
    public string username { get; set; } = string.Empty;

    public string email { get; set; } = string.Empty;

    public string password { get; set; } = string.Empty;
}