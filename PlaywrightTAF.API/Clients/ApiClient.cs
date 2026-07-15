using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.Core.Logging;
using RestSharp;
using Serilog;
using System.Diagnostics;

namespace PlaywrightTAF.API.Clients;

public abstract class ApiClient
{
    private static readonly ILogger Logger = LogProvider.ForContext<ApiClient>();

    protected readonly RestClient Client;

    protected ApiClient()
    {
        Client = new RestClient(ConfigurationReader.Current.ApiBaseUrl);
    }

    protected async Task<RestResponse<T>> ExecuteAsync<T>(RestRequest request)
        where T : class
    {
        var stopwatch = Stopwatch.StartNew();
        Logger.Information("Sending API request {Method} {Resource}", request.Method, request.Resource);

        RestResponse<T> response = await Client.ExecuteAsync<T>(request);
        stopwatch.Stop();

        Logger.Information(
            "Received API response {StatusCode} for {Method} {Resource} in {ElapsedMilliseconds} ms",
            (int)response.StatusCode,
            request.Method,
            request.Resource,
            stopwatch.ElapsedMilliseconds);

        EnsureSuccessfulResponse(response);

        if (response.Data is null)
        {
            throw new InvalidOperationException($"API response did not contain a '{typeof(T).Name}' body. Content: {response.Content}");
        }

        return response;
    }

    protected async Task<RestResponse> ExecuteAsync(RestRequest request)
    {
        var stopwatch = Stopwatch.StartNew();
        Logger.Information("Sending API request {Method} {Resource}", request.Method, request.Resource);

        RestResponse response = await Client.ExecuteAsync(request);
        stopwatch.Stop();

        Logger.Information(
            "Received API response {StatusCode} for {Method} {Resource} in {ElapsedMilliseconds} ms",
            (int)response.StatusCode,
            request.Method,
            request.Resource,
            stopwatch.ElapsedMilliseconds);

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

        Logger.Error(
            response.ErrorException,
            "API request failed with status {StatusCode} ({StatusDescription}). {Message}",
            (int)response.StatusCode,
            response.StatusCode,
            message);

        throw new InvalidOperationException($"API request failed with status {(int)response.StatusCode} ({response.StatusCode}). {message}");
    }
}
