using CyberPulseUso.Frontend.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace CyberPulseUso.Frontend.AuthenticationProviders;

public class AuthenticationProviderJWT : AuthenticationStateProvider, ILoginService
{
    private readonly IJSRuntime _jSRuntime;
    private readonly HttpClient _httpClient;
    private readonly string _tokenKey;
    private readonly AuthenticationState _anonymous;
    private string? _accessToken;
    private bool _rememberMe;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
    public string? AccessToken => _accessToken;
    public bool RememberMe => _rememberMe;
    private readonly SemaphoreSlim _refreshSemaphore = new(1, 1);
   

    public AuthenticationProviderJWT(IJSRuntime jSRuntime, HttpClient httpClient)
    {
        _jSRuntime = jSRuntime;
        _httpClient = httpClient;
        _tokenKey = "TOKEN_KEY";
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public override Task<AuthenticationState>
        GetAuthenticationStateAsync()
    {
        if (string.IsNullOrWhiteSpace(_accessToken))
            return Task.FromResult(_anonymous);

        return Task.FromResult(
            BuildAuthenticationState(_accessToken));
    }

    // LOGIN: se ejecuta desde el navegador para recibir la cookie.
    public async Task<BrowserHttpResponse> LoginWithCredentialsAsync<T>(string url, T model)
    {
        var result = await BrowserRequestAsync(url, "POST", model);

        if (!result.Ok)
            return result;

        try
        {
            using var document = JsonDocument.Parse(result.Body!);

            var root = document.RootElement;

            var token = root.GetProperty("token").GetString();

            var rememberMe = root.TryGetProperty(
                "rememberMe", out var rememberElement)
                && rememberElement.GetBoolean();

            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException(
                    "El Backend no devolvió un Access Token.");

            SetAccessToken(token, rememberMe);
        }
        catch (Exception ex)
        {
            result.Ok = false;
            result.Body = $"No fue posible procesar el token: {ex.Message}";
        }

        return result;
    }



    // REFRESH: el navegador envía automáticamente la cookie.
    public async Task<bool> RefreshAsync(string? accessTokenUsed = null)
    {
        // Capturar el token antes de esperar el semáforo.
        var tokenBeforeWait = accessTokenUsed ?? _accessToken;

        await _refreshSemaphore.WaitAsync();

        try
        {
            // Si otra solicitud renovó el token mientras esperábamos,
            // no es necesario volver a consultar al Backend.
            if (!string.Equals(
                    _accessToken,
                    tokenBeforeWait,
                    StringComparison.Ordinal))
            {
                return !string.IsNullOrWhiteSpace(_accessToken);
            }

            var result = await BrowserRequestAsync(
                "/api/accounts/Refresh",
                "POST",
                null);

            // Si otra solicitud procesó primero el Refresh Token,
            // intentar nuevamente con la cookie que quedó actualizada.
            if (!result.Ok && result.Status == 409)
            {
                result = await BrowserRequestAsync(
                    "/api/accounts/Refresh",
                    "POST",
                    null);
            }

            if (!result.Ok)
            {
                Console.WriteLine(
                    $"[AUTH REFRESH ERROR] Status: {result.Status} | Body: {result.Body}");

                ClearSession();
                return false;
            }

            using var document = JsonDocument.Parse(result.Body!);

            var root = document.RootElement;

            var token = root.GetProperty("token").GetString();

            var rememberMe =
                root.TryGetProperty("rememberMe", out var rememberElement)
                && rememberElement.GetBoolean();

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new InvalidOperationException(
                    "El Backend no devolvió un Access Token.");
            }

            SetAccessToken(token, rememberMe);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error renovando sesión: {ex}");

            ClearSession();

            return false;
        }
        finally
        {
            _refreshSemaphore.Release();
        }
    }
    // Inicialización después de que Blazor esté interactivo.
    //public async Task InitializeAsync()
    //{
    //    if (string.IsNullOrWhiteSpace(_accessToken))
    //        await RefreshAsync();
    //}


    public async Task InitializeAsync()
    {
        Console.WriteLine("[AUTH] InitializeAsync INICIADO");

        if (string.IsNullOrWhiteSpace(_accessToken))
        {
            Console.WriteLine("[AUTH] No hay Access Token. Iniciando RefreshAsync.");

            var result = await RefreshAsync();

            Console.WriteLine($"[AUTH] Resultado de RefreshAsync: {result}");
        }
        else
        {
            Console.WriteLine("[AUTH] Ya existe un Access Token.");
        }
    }



    // LOGOUT: revoca la familia en el Backend y limpia la sesión local.
    public async Task LogoutAsync()
    {
        try
        {
            await BrowserRequestAsync("/api/accounts/Logout", "POST", null);
        }
        finally
        {
            ClearSession();
        }
    }

    // Compatibilidad con llamadas anteriores.
    public Task LoginAsync(string token)
    {
        SetAccessToken(token, _rememberMe);
        return Task.CompletedTask;
    }

    public void ClearSession()
    {
        _accessToken = null;
        _rememberMe = false;

        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }

    private void SetAccessToken(string token, bool rememberMe)
    {
        _accessToken = token;
        _rememberMe = rememberMe;

        var authState = BuildAuthenticationState(token);

        NotifyAuthenticationStateChanged(
            Task.FromResult(authState));

        Console.WriteLine($"[Auth] Access Token actualizado: {DateTime.Now:HH:mm:ss}");
    }

    private AuthenticationState BuildAuthenticationState(string token)
    {
        var claims = ParseClaimsFromJWT(token);

        return new AuthenticationState(
            new ClaimsPrincipal(
                new ClaimsIdentity(claims, "jwt")));
    }

    private IEnumerable<Claim> ParseClaimsFromJWT(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        var jwt = handler.ReadJwtToken(token);

        return jwt.Claims;
    }

    private async Task<BrowserHttpResponse> BrowserRequestAsync(string url, string method, object? model)
    {
        var absoluteUrl = Uri.TryCreate(url, UriKind.Absolute, out var uri)
                ? uri.ToString()
                : new Uri(_httpClient.BaseAddress!, url).ToString();

        return await _jSRuntime.InvokeAsync<BrowserHttpResponse>(
            "cyberPulseAuth.request",
            absoluteUrl,
            method,
            model);
    }

}