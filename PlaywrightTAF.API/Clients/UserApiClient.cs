using PlaywrightTAF.API.Endpoints;
using PlaywrightTAF.Core.Models;
using RestSharp;

namespace PlaywrightTAF.API.Clients;

public class UserApiClient : ApiClient
{
    public async Task<UserResponse?> GetCurrentUserAsync(string token)
    {
        var request = new RestRequest(ApiEndpoints.CurrentUser, Method.Get);

        request.AddHeader("Authorization", $"Token {token}");

        var response = await ExecuteAsync<UserResponse>(request);

        return response.Data;
    }
}
