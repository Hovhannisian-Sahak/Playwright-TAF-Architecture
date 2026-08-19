using PlaywrightTAF.API.Endpoints;
using PlaywrightTAF.API.ResponseModels;
using RestSharp;

namespace PlaywrightTAF.API.Clients;

public class UserApiClient : ApiClient
{
    public async Task<UserResponse?> GetCurrentUserAsync(string token)
    {
        var response = await ExecuteAsync<UserResponse>(CreateCurrentUserRequest(token));

        return response.Data;
    }

    public Task<RestResponse<UserResponse>> SendGetCurrentUserAsync(string token)
    {
        return ExecuteRawAsync<UserResponse>(CreateCurrentUserRequest(token));
    }

    private static RestRequest CreateCurrentUserRequest(string token)
    {
        var request = new RestRequest(ApiEndpoints.CurrentUser, Method.Get);

        request.AddHeader("Authorization", $"Token {token}");

        return request;
    }
}
