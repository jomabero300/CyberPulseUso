using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Shared.EntitiesDTO.Gene;

namespace CyberPulseUso.Mobile.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private const string LoginEndpoint = "api/Accounts/Login";
    private const string LogoutEndpoint = "api/Accounts/Logout";

    private readonly IApiRepository _repository;
    private readonly ITokenService _tokenService;
    private readonly ITokenRefreshService _tokenRefreshService;

    public AuthenticationService(
        IApiRepository repository,
        ITokenService tokenService,
        ITokenRefreshService tokenRefreshService)
    {
        _repository = repository;
        _tokenService = tokenService;
        _tokenRefreshService = tokenRefreshService;
    }

    public async Task<TokenDTO?> LoginAsync(LoginDTO login)
    {
        await _tokenService.ClearAsync();

        var response = await _repository.PostAsync<LoginDTO, TokenDTO>(LoginEndpoint, login);

        if (response is null || string.IsNullOrWhiteSpace(response.Token))
        {
            return null;
        }

        await _tokenService.SaveAccessTokenAsync(response.Token, response.Expiration);

        await _tokenService.SaveRememberMeAsync(login.RememberMe);

        if (login.RememberMe)
        {
            await _tokenService.PersistRefreshCookieAsync();
        }

        return response;
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _repository.PostAsync<object, object?>(LogoutEndpoint, new { });
        }
        finally
        {
            await _tokenService.ClearAsync();
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _tokenService.GetAccessTokenAsync();
        var expiration = await _tokenService.GetExpirationAsync();

        if (string.IsNullOrWhiteSpace(token) || !expiration.HasValue)
            return false;

        // El JWT todavía está vigente.
        if (expiration.Value > DateTime.UtcNow)
            return true;

        // El JWT venció: intentar renovar solamente
        // si RememberMe está activo.
        if (!await _tokenService.GetRememberMeAsync())
            return false;

        return await _tokenRefreshService.RefreshAsync();
    }
}