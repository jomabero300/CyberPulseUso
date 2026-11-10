using CyberPulseUso.Mobile.Configuration;
using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace CyberPulseUso.Mobile.Services.Implementations;

public class TokenRefreshService : ITokenRefreshService
{
    private readonly ITokenService _tokenService;
    private readonly HttpClient _httpClient;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public TokenRefreshService(CookieContainer cookieContainer,ITokenService tokenService)
    {
        _tokenService = tokenService;

        var handler = new HttpClientHandler
        {
            CookieContainer = cookieContainer,
            UseCookies = true
        };

        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(ApiConfiguration.BaseUrl),
            Timeout = TimeSpan.FromSeconds(ApiConfiguration.TimeoutSeconds)
        };
    }

    public async Task<bool> RefreshAsync()
    {
        await _semaphore.WaitAsync();

        try
        {
            if (!await _tokenService.GetRememberMeAsync())
                return false;


            var refreshCookie = await _tokenService.GetRefreshCookieAsync();

            if (string.IsNullOrWhiteSpace(refreshCookie))
                return false;

            using var response = await _httpClient.PostAsync("api/Accounts/Refresh",null);

            var responseContent =await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return false;


            var tokenDTO =JsonSerializer.Deserialize<TokenDTO>(responseContent,JsonOptions);


            if (tokenDTO is null || string.IsNullOrWhiteSpace(tokenDTO.Token))
            {
                return false;
            }

            await _tokenService.SaveAccessTokenAsync(tokenDTO.Token,tokenDTO.Expiration);

            // La respuesta ya habrá actualizado CookieContainer
            // con el nuevo Set-Cookie.
            await _tokenService.PersistRefreshCookieAsync();

            return true;
        }
        catch (HttpRequestException )
        {
            return false;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
        catch (Exception )
        {
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}