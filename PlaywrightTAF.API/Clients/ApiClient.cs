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
        return await Client.ExecuteAsync<T>(request);
    }

    protected async Task<RestResponse> ExecuteAsync(RestRequest request)
    {
        return await Client.ExecuteAsync(request);
    }
}