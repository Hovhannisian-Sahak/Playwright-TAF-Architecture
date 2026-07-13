using PlaywrightTAF.API.Clients;
using PlaywrightTAF.Core.Authentication;
using PlaywrightTAF.Core.Models;

namespace PlaywrightTAF.API.Services;

public class UserService
{
    private readonly TokenProvider _tokenProvider;
    private readonly UserApiClient _userClient;

    public UserService(UserApiClient userClient, TokenProvider tokenProvider)
    {
        _userClient = userClient;
        _tokenProvider = tokenProvider;
    }

    public async Task<UserData> GetCurrentUser(string token)
    {
        var response = await _userClient.GetCurrentUserAsync(token);

        return response!.user;
    }

    public Task<UserData> GetCurrentUser()
    {
        return GetCurrentUser(_tokenProvider.GetToken());
    }
}
