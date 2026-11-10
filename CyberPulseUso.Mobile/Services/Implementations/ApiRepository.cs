using CyberPulseUso.Mobile.Services.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace CyberPulseUso.Mobile.Services.Implementations;

public class ApiRepository : IApiRepository
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

    public ApiRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        return SendAsync<T>(request, cancellationToken);
    }

    public Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(data, options: _jsonOptions)
        };

        return SendAsync<TResponse>(request, cancellationToken);
    }

    public Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest data, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = JsonContent.Create(data, options: _jsonOptions)
        };

        return SendAsync<TResponse>(request, cancellationToken);
    }

    public async Task DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, url);

        using var response = await _httpClient.SendAsync(
            request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(
                cancellationToken);

            throw new HttpRequestException(
                $"Error HTTP {(int)response.StatusCode}: {error}",
                null,
                response.StatusCode);
        }
    }

    private async Task<T?> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using (request)
        using (var response = await _httpClient.SendAsync(
            request, cancellationToken))
        {
            var content = await response.Content.ReadAsStringAsync(
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Error HTTP {(int)response.StatusCode}: {content}",
                    null,
                    response.StatusCode);
            }

            if (string.IsNullOrWhiteSpace(content))
                return default;

            return JsonSerializer.Deserialize<T>(
                content, _jsonOptions);
        }
    }

}