using PlaywrightTAF.API.Clients;
using PlaywrightTAF.Core.RequestModels;

namespace PlaywrightTAF.Core.Authentication;

public class TokenProvider
{
    private readonly AuthApiClient _authClient;


    private string? _token;



    public TokenProvider(
        AuthApiClient authClient)
    {
        _authClient = authClient;
    }




    public async Task<string> GetTokenAsync()
    {

        if (_token != null)
            return _token;



        var response =
            await _authClient.LoginAsync(
                new LoginRequest
                {
                    user = new UserLogin
                    {
                        email =
                            "test@test.com",

                        password =
                            "password"
                    }
                });



        _token =
            response!.user.token;


        return _token;
    }
}