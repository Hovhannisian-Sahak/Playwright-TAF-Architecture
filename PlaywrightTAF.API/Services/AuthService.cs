using PlaywrightTAF.API.Clients;
using PlaywrightTAF.Core.RequestModels;

namespace PlaywrightTAF.API.Services;

public class AuthService
{
    private readonly AuthApiClient _authClient;

    public AuthService(AuthApiClient authClient)
    {
        _authClient = authClient;
    }

    public async Task<string> Login(string email, string password)
    {
        var response = await _authClient.LoginAsync(
            new LoginRequest
            {
                user = new UserLogin
                {
                    email = email,
                    password = password
                }
            });

        return response!.user.token;
    }

    public async Task<string> Register(string username, string email, string password)
    {
        var response = await _authClient.RegisterAsync(
            new RegisterRequest
            {
                user = new UserRegister
                {
                    username = username,
                    email = email,
                    password = password
                }
            });

        return response!.user.token;
    }
}
