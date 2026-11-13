using CyberPulseUso.Mobile.Models;

namespace CyberPulseUso.Mobile.Services.Interfaces;

public interface ITokenService
{
    Task SaveAccessTokenAsync(string token, DateTime expiration);

    Task<string?> GetAccessTokenAsync();

    Task<DateTime?> GetExpirationAsync();

    Task SaveRefreshCookieAsync(string cookieValue);

    Task<string?> GetRefreshCookieAsync();

    Task<bool> GetRememberMeAsync();

    Task SaveRememberMeAsync(bool rememberMe);

    Task ClearAsync();

    Task PersistRefreshCookieAsync();

    Task<UserSession?> GetUserSessionAsync();
}