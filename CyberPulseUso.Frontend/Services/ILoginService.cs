using CyberPulseUso.Frontend.Helpers;

namespace CyberPulseUso.Frontend.Services;

public interface ILoginService
{
    Task LoginAsync(string token);

    Task<BrowserHttpResponse> LoginWithCredentialsAsync<T>(
        string url, T model);

    Task<bool> RefreshAsync(string? accessTokenUsed = null);

    Task InitializeAsync();

    Task LogoutAsync();

    bool RememberMe { get; }

}
