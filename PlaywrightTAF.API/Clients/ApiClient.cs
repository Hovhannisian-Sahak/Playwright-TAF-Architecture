using PlaywrightTAF.Core.Configuration;
using RestSharp;

namespace PlaywrightTAF.API.Clients;

public abstract class ApiClient
{
    protected readonly RestClient Client;

    protected ApiClient()
    {
        Client = new RestClient(ConfigurationReader.Current.ApiBaseUrl);
    }

    protected async Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request)
        where T : class
    {
        RestResponse<T> response = await Client.ExecuteAsync<T>(request);

        EnsureSuccessfulResponse(response);

        if (response.Data is null)
        {
            throw new InvalidOperationException($"API response did not contain a '{typeof(T).Name}' body. Content: {response.Content}");
        }

        return response;
    }

    protected async Task<RestResponse> ExecuteAsync(RestRequest request)
    {
        RestResponse response = await Client.ExecuteAsync(request);

        EnsureSuccessfulResponse(response);

        return response;
    }

    private static void EnsureSuccessfulResponse(RestResponse response)
    {
        if (response.IsSuccessful)
        {
            return;
        }

        string message = string.IsNullOrWhiteSpace(response.Content)
            ? response.ErrorMessage ?? "No response content."
            : response.Content;

        throw new InvalidOperationException($"API request failed with status {(int)response.StatusCode} ({response.StatusCode}). {message}");
    }
}
