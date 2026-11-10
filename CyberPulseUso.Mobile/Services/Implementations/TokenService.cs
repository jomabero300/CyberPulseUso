using CyberPulseUso.Mobile.Configuration;
using CyberPulseUso.Mobile.Services.Interfaces;
using System.Globalization;
using System.Net;

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

    public async Task SaveAccessTokenAsync( string token, DateTime expiration)
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
        await SecureStorage.Default.SetAsync(RememberMeKey,rememberMe.ToString());

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

        var cookie = new Cookie(RefreshCookieName,cookieValue,"/api")
        {
            Secure = true,
            HttpOnly = true
        };

        _cookieContainer.Add(_apiUri, cookie);
    }

    private void RemoveRefreshCookie()
    {
        var expiredCookie = new Cookie(RefreshCookieName,string.Empty,"/")
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
}