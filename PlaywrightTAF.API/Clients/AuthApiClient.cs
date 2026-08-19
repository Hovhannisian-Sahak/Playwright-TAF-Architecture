using PlaywrightTAF.API.Endpoints;
using PlaywrightTAF.API.ResponseModels;
using PlaywrightTAF.API.RequestModels;
using RestSharp;

namespace PlaywrightTAF.API.Clients;

public class AuthApiClient : ApiClient
{
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var response = await ExecuteAsync<AuthResponse>(CreateLoginRequest(request));

        return response.Data;
    }

    public Task<RestResponse<AuthResponse>> SendLoginAsync(LoginRequest request)
    {
        return ExecuteRawAsync<AuthResponse>(CreateLoginRequest(request));
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var response = await ExecuteAsync<AuthResponse>(CreateRegisterRequest(request));

        return response.Data;
    }

    public Task<RestResponse<AuthResponse>> SendRegisterAsync(RegisterRequest request)
    {
        return ExecuteRawAsync<AuthResponse>(CreateRegisterRequest(request));
    }

    private static RestRequest CreateLoginRequest(LoginRequest request)
    {
        var restRequest = new RestRequest(ApiEndpoints.Login, Method.Post);

        restRequest.AddJsonBody(request);

        return restRequest;
    }

    private static RestRequest CreateRegisterRequest(RegisterRequest request)
    {
        var restRequest = new RestRequest(ApiEndpoints.Register, Method.Post);

        restRequest.AddJsonBody(request);

        return restRequest;
    }
}
