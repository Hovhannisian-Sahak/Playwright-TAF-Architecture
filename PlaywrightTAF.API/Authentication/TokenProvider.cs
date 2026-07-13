using PlaywrightTAF.API.Clients;
using PlaywrightTAF.API.Services;
using PlaywrightTAF.Core.RequestModels;

namespace PlaywrightTAF.Core.Authentication;

public class TokenProvider
{
    private string? _token;
    public async Task<string> GetTokenAsync()
    {
        if (_token != null)
        {
            return _token;
        }

        var auth = new AuthService(new AuthApiClient());
        _token = await auth.Login("test@mail.com", "Password123");
        return _token;
    }
}