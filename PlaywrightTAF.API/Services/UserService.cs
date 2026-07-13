using PlaywrightTAF.API.Clients;
using PlaywrightTAF.Core.Models;

namespace PlaywrightTAF.API.Services;

public class UserService
{
    private readonly UserApiClient _userClient;

    public UserService(UserApiClient userClient)
    {
        _userClient = userClient;
    }

    public async Task<UserData> GetCurrentUser(string token)
    {
        var response =
            await _userClient.GetCurrentUserAsync(token);

        return response!.user;
    }
}
