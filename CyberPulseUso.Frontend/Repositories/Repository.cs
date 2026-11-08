using CyberPulseUso.Frontend.AuthenticationProviders;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CyberPulseUso.Frontend.Repositories;

public class Repository : IRepository
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationProviderJWT _authenticationProvider;

    private JsonSerializerOptions _jsonDefaultOptions => new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    public Repository(HttpClient httpClient, AuthenticationProviderJWT authenticationProvider)
    {
        _httpClient = httpClient;
        _authenticationProvider = authenticationProvider;
    }


    // GET con respuesta tipada
    public async Task<HttpResponseWrapper<T>> GetAsync<T>(string url)
    {
        var responseHttp = await SendWithRefreshAsync(
            () => new HttpRequestMessage(HttpMethod.Get, url));

        if (responseHttp.IsSuccessStatusCode)
        {
            var response = await UnserializeAnswer<T>(responseHttp);

            return new HttpResponseWrapper<T>(
                response, false, responseHttp);
        }

        return new HttpResponseWrapper<T>(
            default, true, responseHttp);
    }

    // POST sin respuesta tipada
    public async Task<HttpResponseWrapper<object>> PostAsync<T>(
        string url, T model)
    {
        var responseHttp = await SendWithRefreshAsync(
            () => CreateRequest(HttpMethod.Post, url, model));

        return new HttpResponseWrapper<object>(
            null, !responseHttp.IsSuccessStatusCode, responseHttp);
    }

    // POST con respuesta tipada
    public async Task<HttpResponseWrapper<TActionResponse>>PostAsync<T, TActionResponse>(string url, T model)
    {
        var responseHttp = await SendWithRefreshAsync(
            () => CreateRequest(HttpMethod.Post, url, model));

        if (responseHttp.IsSuccessStatusCode)
        {
            var response =
                await UnserializeAnswer<TActionResponse>(responseHttp);

            return new HttpResponseWrapper<TActionResponse>(
                response, false, responseHttp);
        }

        return new HttpResponseWrapper<TActionResponse>(
            default, true, responseHttp);
    }

    // DELETE
    public async Task<HttpResponseWrapper<object>> DeleteAsync(string url)
    {
        var responseHttp = await SendWithRefreshAsync(
            () => new HttpRequestMessage(HttpMethod.Delete, url));

        return new HttpResponseWrapper<object>(
            null, !responseHttp.IsSuccessStatusCode, responseHttp);
    }

    // PUT sin respuesta tipada
    public async Task<HttpResponseWrapper<object>> PutAsync<T>(string url, T model)
    {
        var responseHttp = await SendWithRefreshAsync(
            () => CreateRequest(HttpMethod.Put, url, model));

        return new HttpResponseWrapper<object>(
            null, !responseHttp.IsSuccessStatusCode, responseHttp);
    }

    // PUT con respuesta tipada
    public async Task<HttpResponseWrapper<TActionResponse>>PutAsync<T, TActionResponse>(string url, T model)
    {
        var responseHttp = await SendWithRefreshAsync(
            () => CreateRequest(HttpMethod.Put, url, model));

        if (responseHttp.IsSuccessStatusCode)
        {
            var response =
                await UnserializeAnswer<TActionResponse>(responseHttp);

            return new HttpResponseWrapper<TActionResponse>(
                response, false, responseHttp);
        }

        return new HttpResponseWrapper<TActionResponse>(
            default, true, responseHttp);
    }

    // GET sin respuesta tipada
    public async Task<HttpResponseWrapper<object>> GetAsync(string url)
    {
        var responseHttp = await SendWithRefreshAsync(
            () => new HttpRequestMessage(HttpMethod.Get, url));

        return new HttpResponseWrapper<object>(
            null, !responseHttp.IsSuccessStatusCode, responseHttp);
    }

    // GET de archivos
    public async Task<HttpResponseWrapper<byte[]>> GetBytesAsync(string url)
    {
        var responseHttp = await SendWithRefreshAsync(
            () => new HttpRequestMessage(HttpMethod.Get, url));

        if (responseHttp.IsSuccessStatusCode)
        {
            var bytes = await responseHttp.Content.ReadAsByteArrayAsync();

            return new HttpResponseWrapper<byte[]>(
                bytes, false, responseHttp);
        }

        return new HttpResponseWrapper<byte[]>(
            null, true, responseHttp);
    }

    // -------------------------------------------------------
    // MÉTODOS PRIVADOS
    // -------------------------------------------------------

    private HttpRequestMessage CreateRequest<T>(
        HttpMethod method, string url, T model)
    {
        var json = JsonSerializer.Serialize(model);

        var content = new StringContent(
            json, Encoding.UTF8, "application/json");

        return new HttpRequestMessage(method, url)
        {
            Content = content
        };
    }

    private async Task<HttpResponseMessage> SendWithRefreshAsync(Func<HttpRequestMessage> requestFactory)
    {
        // Capturar el Access Token que utilizará la primera solicitud.
        var accessTokenUsed = _authenticationProvider.AccessToken;

        var response = await SendOnceAsync(
            requestFactory,
            accessTokenUsed);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
            return response;

        // Intentar renovar utilizando el token que recibió el 401.
        var refreshed = await _authenticationProvider.RefreshAsync(
            accessTokenUsed);

        if (!refreshed)
            return response;

        response.Dispose();

        // Reintentar con el nuevo Access Token.
        var retryResponse = await SendOnceAsync(
            requestFactory,
            _authenticationProvider.AccessToken);

        if (retryResponse.StatusCode == HttpStatusCode.Unauthorized)
        {
            _authenticationProvider.ClearSession();
        }

        return retryResponse;
    }

    private async Task<HttpResponseMessage> SendOnceAsync(Func<HttpRequestMessage> requestFactory,string? accessToken)
    {
        using var request = requestFactory();

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var requestPath = request.RequestUri is { IsAbsoluteUri: true } uri
            ? uri.AbsolutePath
            : request.RequestUri?.OriginalString;

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            // Esperar inicialmente solo los encabezados de respuesta.
            var response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead);

            stopwatch.Stop();

            Console.WriteLine(
                $"[API] {request.Method} {requestPath} => " +
                $"{(int)response.StatusCode} {response.StatusCode} " +
                $"| Tiempo: {stopwatch.ElapsedMilliseconds} ms");

            return response;
        }
        catch (TaskCanceledException ex)
        {
            stopwatch.Stop();

            Console.WriteLine(
                $"[API TIMEOUT/CANCELACIÓN] {request.Method} {requestPath} " +
                $"| Tiempo: {stopwatch.ElapsedMilliseconds} ms " +
                $"| Mensaje: {ex.Message}");

            throw;
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();

            Console.WriteLine(
                $"[API ERROR] {request.Method} {requestPath} " +
                $"| Tiempo: {stopwatch.ElapsedMilliseconds} ms " +
                $"| Error: {ex.Message}");

            throw;
        }
    }
    private async Task<T> UnserializeAnswer<T>(
        HttpResponseMessage responseHttp)
    {
        var response = await responseHttp.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(
            response, _jsonDefaultOptions)!;
    }
}
