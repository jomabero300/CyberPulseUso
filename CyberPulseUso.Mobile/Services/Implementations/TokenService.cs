using CyberPulseUso.Mobile.Configuration;
using CyberPulseUso.Mobile.Models;
using CyberPulseUso.Mobile.Services.Interfaces;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text.Json;

namespace CyberPulseUso.Mobile.Services.Implementations;

public class TokenService : ITokenService
{
    private const string TokenKey = "ACCESS_TOKEN";
    private const string ExpirationKey = "TOKEN_EXPIRATION";
    private const string RefreshCookieKey = "REFRESH_COOKIE";
    private const string RememberMeKey = "REMEMBER_ME";

    private const string RefreshCookieName = "CYBERPULSE_REFRESH";

    private readonly CookieContainer _cookieContainer;
    private readonly Uri _apiUri;

    public TokenService(CookieContainer cookieContainer)
    {
        _cookieContainer = cookieContainer;
        _apiUri = new Uri(ApiConfiguration.BaseUrl);
    }

    public async Task SaveAccessTokenAsync(string token, DateTime expiration)
    {
        await SecureStorage.Default.SetAsync(TokenKey, token);

        await SecureStorage.Default.SetAsync(
            ExpirationKey,
            expiration.ToString("O", CultureInfo.InvariantCulture));
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        return await SecureStorage.Default.GetAsync(TokenKey);
    }

    public async Task<DateTime?> GetExpirationAsync()
    {
        var value = await SecureStorage.Default.GetAsync(ExpirationKey);

        if (string.IsNullOrWhiteSpace(value))
            return null;

        return DateTime.TryParse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out var expiration)
                ? expiration
                : null;
    }

    public async Task SaveRefreshCookieAsync(string cookieValue)
    {
        if (string.IsNullOrWhiteSpace(cookieValue))
            return;

        await SecureStorage.Default.SetAsync(
            RefreshCookieKey, cookieValue);

        RestoreRefreshCookie(cookieValue);
    }

    public async Task<string?> GetRefreshCookieAsync()
    {
        var value = await SecureStorage.Default.GetAsync(RefreshCookieKey);

        if (!string.IsNullOrWhiteSpace(value))
            RestoreRefreshCookie(value);

        return value;
    }

    public async Task<bool> GetRememberMeAsync()
    {
        var value = await SecureStorage.Default.GetAsync(RememberMeKey);

        return bool.TryParse(value, out var rememberMe) && rememberMe;
    }

    public async Task SaveRememberMeAsync(bool rememberMe)
    {
        await SecureStorage.Default.SetAsync(RememberMeKey, rememberMe.ToString());

        if (!rememberMe)
        {
            SecureStorage.Default.Remove(RefreshCookieKey);
            RemoveRefreshCookie();
        }
    }

    public async Task ClearAsync()
    {
        SecureStorage.Default.Remove(TokenKey);
        SecureStorage.Default.Remove(ExpirationKey);
        SecureStorage.Default.Remove(RefreshCookieKey);
        SecureStorage.Default.Remove(RememberMeKey);

        RemoveRefreshCookie();
    }

    private void RestoreRefreshCookie(string cookieValue)
    {
        RemoveRefreshCookie();

        var cookie = new Cookie(RefreshCookieName, cookieValue, "/api")
        {
            Secure = true,
            HttpOnly = true
        };

        _cookieContainer.Add(_apiUri, cookie);
    }

    private void RemoveRefreshCookie()
    {
        var expiredCookie = new Cookie(RefreshCookieName, string.Empty, "/")
        {
            Secure = true,
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(-1)
        };

        _cookieContainer.Add(_apiUri, expiredCookie);
    }

    public async Task PersistRefreshCookieAsync()
    {
        var refreshUri = new Uri(_apiUri, "api/Accounts/Refresh");
        var cookies = _cookieContainer.GetCookies(refreshUri);
        var cookie = cookies[RefreshCookieName];

        if (cookie is null || string.IsNullOrWhiteSpace(cookie.Value))
        {
            return;
        }

        await SaveRefreshCookieAsync(cookie.Value);
    }




    public async Task<UserSession?> GetUserSessionAsync()
    {
        var token = await GetAccessTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
            return null;

        try
        {
            var parts = token.Split('.');

            if (parts.Length != 3)
                return null;

            var payload = parts[1]
                .Replace('-', '+')
                .Replace('_', '/');

            payload = payload.PadRight(
                payload.Length + (4 - payload.Length % 4) % 4,
                '=');

            var jsonBytes = Convert.FromBase64String(payload);

            using var document = JsonDocument.Parse(jsonBytes);

            var root = document.RootElement;

            string GetClaim(params string[] names)
            {
                foreach (var property in root.EnumerateObject())
                {
                    if (names.Any(name =>
                        string.Equals(property.Name, name,
                            StringComparison.OrdinalIgnoreCase)))
                    {
                        return property.Value.ToString();
                    }
                }

                return string.Empty;
            }

            var session = new UserSession
            {
                Email = GetClaim(
                   "unique_name",
                   "name",
                   "email",
                   "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"),

                FirstName = GetClaim("FirstName", "FrstName"),

                LastName = GetClaim("LastName"),

                Photo = GetClaim("Photo"),

                UserType = GetClaim(
                   "role",
                   "Role",
                   "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"),

                ChargeName = GetClaim("ChargeName"),

                CountryId = int.TryParse(GetClaim("CountryId"),out var countryId) ? countryId : 0
            };

            Debug.WriteLine("===== DATOS DE USER SESSION =====");
            Debug.WriteLine($"Email: [{session.Email}]");
            Debug.WriteLine($"UserType: [{session.UserType}]");
            Debug.WriteLine($"ChargeName: [{session.ChargeName}]");
            Debug.WriteLine($"FullName: [{session.FullName}]");
            Debug.WriteLine("=================================");

            Debug.WriteLine($"¿Tiene fotografía?: {!string.IsNullOrWhiteSpace(session.Photo)}");
            Debug.WriteLine($"Longitud de fotografía: {session.Photo?.Length ?? 0}");

            return session;
        }
        catch (Exception ex) when (
            ex is FormatException ||
            ex is JsonException ||
            ex is ArgumentException)
        {
            Debug.WriteLine($"Error leyendo sesión: {ex}");
            return null;
        }
    }
}