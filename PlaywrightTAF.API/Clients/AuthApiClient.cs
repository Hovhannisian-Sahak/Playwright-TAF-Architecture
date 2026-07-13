using PlaywrightTAF.API.Endpoints;
using PlaywrightTAF.Core.Models;
using PlaywrightTAF.Core.RequestModels;
using RestSharp;

namespace PlaywrightTAF.API.Clients;

public class AuthApiClient : ApiClient
{
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var restRequest = new RestRequest(ApiEndpoints.Login, Method.Post);

        restRequest.AddJsonBody(request);

        var response = await ExecuteAsync<AuthResponse>(restRequest);

        return response.Data;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var restRequest = new RestRequest(ApiEndpoints.Register, Method.Post);

        restRequest.AddJsonBody(request);

        var response = await ExecuteAsync<AuthResponse>(restRequest);

        return response.Data;
    }
}
